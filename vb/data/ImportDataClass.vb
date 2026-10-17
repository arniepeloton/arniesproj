Imports System.Threading
Imports C1.C1Zip
Imports System.Net
Imports Newtonsoft.Json
Imports WebProj.WebProj
Imports WinSCP
Imports System.IO
Imports System.Web.Services.Description

Public Class ImportDataClass
    Implements IDisposable

    Public InputFilename As String
    Public TransImportsID As String
    Public OrgID As String
    Public Uuid As String
    Public DataImportLog As DataImportLog
    ReadOnly mBusiness As Business
    ReadOnly mFolder As String
    Dim mFreeUser As Boolean ' MobizLibrarySubscription.FreeUser
    Dim mFromFtp As Boolean

    Dim mDone As Boolean
    Public DeviceID As String
    Dim mThread As Threading.Thread
    Public AllowReimportNonZeroInvoice As Boolean
    Public ImportInvoiceOnly As Boolean
    Dim HasOnCreditPayment As Boolean
    Dim IsEmailReceipt As Boolean
    Dim IsInsertToPaymentsTable As Boolean
    Dim CustomerEmailAddresses As String
    'Public InvoiceNumbersToImport As String
    Public InvoiceIDToImport As List(Of String)
    Public EnableLogs As Boolean
    Const mLogFile As String = "ImportDataClass"
    ''' <summary>
    ''' optional. used for debugging only
    ''' </summary>
    ''' <remarks></remarks>
    Public UploadsFolder As String
    Event Done()
    Dim mFowstID As String
    Dim pMustRecacheInvoice As Boolean
    Dim mLogFilename As String
    Dim mItemModifiersCount As Integer
    Dim mSalesRepCount As Integer
    Dim mItemRebatesCount As Integer
    Dim mItemSerialsCount As Integer
    Dim mInvoiceDiscountCount As Integer
    Dim mSeniorPwdCount As Integer
    Dim mInvoiceChargesCount As Integer
    Dim mItemsVoidedCount As Integer
    Dim mPaymentCount As Integer
    Dim mSwAll As Stopwatch
    Dim mSwUpdateZReadCache As New Stopwatch
    Dim mSwUpdateEJournalCache As New Stopwatch
    Dim mSwCacheInvoicesWithCredit As New Stopwatch
    Dim mPaymentStopWatch As New Stopwatch
    Dim mCachedInvoices As New List(Of String)
    Dim mIsWebPos As Boolean
    Dim mDatabaseName As String
    Dim mFastFlushingMode As Boolean
    Dim mConsolidationMode As Boolean
    Const mFastFlushingBatchSize As Integer = 10
    Dim mBusinessSource As Business ' source business for consolidation mode
    Dim mBusiness1stInstance As Business ' source business for consolidation mode
    Public IsSuccess As Boolean
    Dim mIgnoreZReadCacheError As Boolean
    Dim mAllowImportDrafts As Boolean ' set in device by flush_drafts.txt and server "settings\FlushDraftsOrgIDs.txt"
    Dim mMobizAppVersion As String

    Public Shared Status As String
    Public Shared StatusLastUpdated As DateTime
    Public Shared LastSuccess As String
    Public Shared LastSuccessDate As DateTime
    Public Shared FirstStarted As DateTime = Now

    Public Sub New()

        mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
#If DEBUG Then
        mBusiness.ProData.ExecuteSQL("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED")
#End If
        If Now.Date = #4/27/2023# Then
            mBusiness.ProData.LogFolder = "temp\logs\SQL\" & DateTimeStamp()
        End If
        mFolder = Common.ServerPath & "temp\pending import data replies\"
        If UploadsFolder Is Nothing Then
            UploadsFolder = Common.ServerPath & "temp\Uploads\"
        End If

    End Sub

    Public Sub New(vUseFastFlushing As Boolean, vDatabaseName As String, vConsolidationMode As Boolean)
        ' vDatabaseName: for fast flushing, set database name here

        mFastFlushingMode = vUseFastFlushing
        mConsolidationMode = vConsolidationMode
        If vUseFastFlushing OrElse mConsolidationMode Then
            mDatabaseName = vDatabaseName
            Dim pFilename = Common.ServerPath & "settings\FlushingDb.data"
            If IO.File.Exists(pFilename) Then
                Dim pFlushingConnectionInfo = New ConnectionInfo()
                pFlushingConnectionInfo.Filename = pFilename
                If pFlushingConnectionInfo.Open() Then
                    If mConsolidationMode Then
                        mBusinessSource = New Business(ProData.CreateFromConnectionInfo(pFlushingConnectionInfo))
                    Else
                        mBusiness = New Business(ProData.CreateFromConnectionInfo(pFlushingConnectionInfo))
                        mBusiness1stInstance = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                    End If
                End If
            End If
        End If

        If mBusiness Is Nothing Then
            mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
        End If

        mFolder = Common.ServerPath & "temp\pending import data replies\"
        If UploadsFolder Is Nothing Then
            UploadsFolder = Common.ServerPath & "temp\Uploads\"
        End If

    End Sub

    Public Sub New(vBusiness As Business)
        ' use for zread caching
        mBusiness = vBusiness

    End Sub

    Private Shared mFastFlushingOrgIDs As List(Of String)

    Public Shared Function FastFlushingOrgIDs() As List(Of String)

        If mFastFlushingOrgIDs Is Nothing Then
            mFastFlushingOrgIDs = New List(Of String)
            'mFastFlushingOrgIDs.Add(ReadyGuid(modClient.DemoCorpOrgID))
        End If

        Return mFastFlushingOrgIDs

    End Function

    Public ReadOnly Property TimeStamp As String
        Get

            Return IO.Path.GetFileNameWithoutExtension(InputFilename)

        End Get
    End Property

    Function GetAllowImportDrafts() As Boolean

        Dim pFilename = Common.ServerPath & "settings\FlushDraftsOrgIDs.txt"
        Dim pFileText = FileText(pFilename)
        If String.IsNullOrWhiteSpace(pFileText) Then
            Return False
        End If

        pFileText = pFileText.ToUpper()
        Dim pSplits = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Dim pOut = pSplits.Contains(OrgID.ToUpper())

        Return pOut

    End Function

    Public Sub Start()

        Dim pDR As DataRow

        If CanFlushNow() = False Then
            Return
        End If
        mSwAll = Stopwatch.StartNew()

        If EnableLogs Then AppendToLog("vbD957WV6CVT! start: " & InputFilename, , mLogFile)

        If IsGuid(OrgID) = False Then
            Throw New Exception("OrgID must be a valid GUID before calling Start. vsGM5UAT9R35")
        End If
        'If ReadyGuid(OrgID) = ReadyGuid(modClient.CincoOrgID) Then
        '    Return
        'End If

        If mConsolidationMode = False Then
            If IsGuid(DeviceID) = False Then
                Throw New Exception("DeviceID must be a valid GUID before calling Start. vbH762G76ENN")
            End If
            DeviceID = ReadyGuid(DeviceID)
            If DeviceID = modClient.SportsTownOrgID Then
                AppendToLog("vbV4MA4V32WA! Start", , "ImportDataClass2")
            End If
        End If

        mBusiness.OrgID = OrgID
        mAllowImportDrafts = GetAllowImportDrafts()
        DataImportLog = New DataImportLog

        If mConsolidationMode Then
            mBusinessSource.ProData.ExecuteSQL("USE " & EscapeSqlName(mDatabaseName))
            mBusiness.ProData.ExecuteSQL("USE " & EscapeSqlName(mDatabaseName))
        ElseIf mFastFlushingMode Then
            mBusiness.ProData.ExecuteSQL("USE " & EscapeSqlName(mDatabaseName))
        Else
            pDR = mBusiness.LibraryOrganization.ByID(OrgID)
            If pDR Is Nothing Then
                Throw New Exception("OrgID (" & OrgID & ") point to a non-existing record. vbJBSFSVZZR5")
            End If
            If CStr2(pDR("Database")).Trim = "" Then
                Throw New Exception("OrgID (" & OrgID & ") has no database. vbFDPU3SCMLQ")
            End If
            If mBusiness.ProData.GetDatabases(True, True).Contains(CStr(pDR("Database")).Trim().ToLower()) = False Then
                Throw New Exception("OrgID (" & OrgID & ") point to a non-existing database, " & pDR("Database") & ". vbY5HWN6HWRD")
            End If
            mFreeUser = pDR("FreeUser")
            AddLog("OrgID = " & OrgID & ". Private database name = " & pDR("Database") & ". Result filename = " & GetFilename())

            mBusiness.ProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))
        End If
        If mBusiness1stInstance IsNot Nothing Then
            mBusiness1stInstance.ProData.ExecuteSQL("USE " & EscapeSqlName(mDatabaseName))
        End If
        mIgnoreZReadCacheError = CBool2(FileText(Common.ServerPath & "settings\IgnoreZReadCacheError.txt"))
        AddLog("Changed current database")

        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Start", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
        mSwAll.Restart()

        mThread = New Threading.Thread(CType(
            Sub()
                Dim pResult As String
                Dim pOk As Boolean

                Try
                    StartTransImport(TimeStamp)
                    AddLog("After StartTransImport. ref: vbZCFGHCB3SL!")
                    pOk = Process(pResult)
                    If pOk = False _
                        AndAlso CStr2(pResult).Contains("vb26CQKWA5ZY") Then

                        If String.IsNullOrWhiteSpace(TransImportsID) = False Then
                            Dim pIU = New InsertUpdate("TransImports")
                            pIU("ProcessStart") = "NULL"
                            'pIU("DateReceived") = "DATEADD (ss, 2 * 60, DateReceived)"
                            Dim pSql = pIU.GetUpdate("ID = " & ReadyString(TransImportsID) & vbCrLf &
                                " AND ProcessStart IS NOT NULL" & vbCrLf &
                                " AND ProcessEnd IS NULL")
                            Dim pAffectedCount = mBusiness.ProData.ExecuteSQL(pSql)
                            If pAffectedCount <> 1 Then
                                Console.WriteLine("fatay: " & pAffectedCount & ", vb56BGWN54T3")
                            End If
                        End If
                        GoTo End_Sub
                    End If
                Catch e As Exception
                    Try
                        mBusiness.ProData.RollbackTransaction(True, True)
                        pResult = JsonDictionary.Serialize("Error", e.Message & ";" &
                            GetStackTrace(True, e))
                        UpdateTransImport(TimeStamp, "error", True)
                        AddLog("After UpdateTransImport. ref: vb5HTBC6DJW2!")
                    Catch ex As Exception
                        Debug.WriteLine(ex.Message)
                    End Try
                End Try
                Try
                    mBusiness.ProData.RollbackTransaction(True, True)
                    FileText(GetFilename()) = pResult
                    If pOk Then
                        IsSuccess = True
                        UpdateTransImport(TimeStamp, "ok", True)
                        LastSuccess = UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & ".zip"
                        LastSuccessDate = Now
                    Else
                        If CStr2(pResult).ToLower.Contains("was deadlocked on lock resources with another process and has been chosen as the deadlock victim".ToLower()) _
                            OrElse CStr2(pResult).ToLower.Contains("was deadlocked on lock".ToLower()) _
                            OrElse CStr2(pResult).ToLower.Contains("This SqlTransaction has completed".ToLower()) Then

                            UpdateTransImport(TimeStamp, Nothing, False)
                        Else
                            UpdateTransImport(TimeStamp, "error", False)
                        End If
                        If EnableLogs Then AppendToLog("vbTK7BK5K3YB! not ok: " & InputFilename & "; " & pResult, , mLogFile)
                    End If
                    AddLog("After UpdateTransImport. ref: vbNJ4J6HFFSA!")
                Catch ex As Exception
                    Debug.WriteLine(ex.Message)
                End Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="End of import vbSXL6M99KF9", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
End_Sub:
                Try
                    If mBusiness IsNot Nothing _
                        AndAlso mBusiness.ProData IsNot Nothing Then

                        mBusiness.ProData.Dispose()
                    End If
                Catch ex As Exception
                    AddLog("vb5CAHV2779W! " & ex.Message)
                End Try
                mDone = True
                RaiseEvent Done()
                AddLog("Done(). ref: vbU6NH83WPYZ!")
            End Sub, ThreadStart))
        mThread.Start()

    End Sub

    Public ReadOnly Property IsRunning As Boolean
        Get

            If mThread Is Nothing Then
                Return False
            End If
            Return (mThread.ThreadState = Threading.ThreadState.Running) _
                OrElse (mThread.ThreadState = Threading.ThreadState.WaitSleepJoin)

        End Get
    End Property

    Public Function GetMessage() As String

        Return "Server is still processing imported data."

    End Function

    Public Function IsDone() As Boolean

        Return mDone

    End Function

    Public Function GetFilename() As String

        Return UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & "_result.txt"

    End Function

    Sub AppendToImportedInvoicesFile(vInvoiceID As String)

        AppendToFile(GetImportedInvoicesFilename(), ReadyGuid(vInvoiceID) & vbCrLf)

    End Sub

    Function GetImportedInvoicesFilename() As String

        Return UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & "_ImportedInvoices.txt"

    End Function

    Sub AppendToImportedPaymentInvoicesFile(vInvoiceID As String)

        AppendToFile(GetImportedPaymentInvoicesFilename(), ReadyGuid(vInvoiceID) & vbCrLf)

    End Sub

    Function GetImportedPaymentInvoicesFilename() As String

        Return UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & "_ImportedPaymentInvoices.txt"

    End Function

    Private Function MoveFileIfNeeded(vFilename As String) As Boolean

        If IO.File.Exists(vFilename) Then
            Return True
        End If

        Dim pFolder As String
#If DEBUG Then
        If Common.ServerPath.ToLower() = "E:\dev\debug\Mobiz\WebErp\".ToLower() Then
            pFolder = "E:\dev\debug\Mobiz\WebErp2\"
        Else
            pFolder = "E:\dev\debug\Mobiz\WebErp\"
        End If
#Else
        If Common.ServerPath.ToLower() = "c:\mobiz\" Then
            pFolder = "c:\mobiz2\"
        Else
            pFolder = "c:\mobiz\"
        End If
#End If
        Dim pRelativePath = GetRelativePath(Common.ServerPath, vFilename)
        Dim pFilename = IO.Path.Combine(pFolder, pRelativePath)

        If IO.File.Exists(pFilename) = False Then
            Return False
        End If

        MoveFile(pFilename, vFilename)

        Return True

    End Function

    Private Function Process(ByRef oMessage As String) As Boolean

        Dim pOut As New JsonDictionary
        Dim pZipOut As ProZip
        Dim pError As String
        Dim pFilenameOut As String
        Dim pFilename As String
        'Dim pFranchiseeDataExporter As FranchiseeDataExporter
        Dim pDatabaseName As String

        Dim pInfo As String
        Dim pInfoJson As JsonDictionary

        oMessage = ""
        If False Then
            Throw New Exception("Test exception vbWDVXFXZASB")
        End If

        If False Then
            Threading.Thread.Sleep(60 * 1000)
        End If

        If mConsolidationMode Then
            ImportFrom2ndInstance()
            pOut.Add("Result", "ok")
            oMessage = pOut.Serialize
            Return True
        Else
            pFilename = UploadsFolder & OrgID & "\" & DeviceID & "\" & InputFilename & ".zip"

            ExtractSignatures(pFilename)
            AddLog("ExtractSignatures")
            If EnableLogs Then AppendToLog("vbYSCAYK3M6G! Process " & pFilename, , mLogFile)
            If MoveFileIfNeeded(pFilename) = False Then
                pError = "Zip file does not exist. ref: vb2TRT6T5GUJ!"
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            Dim pZip = New ProZip(pFilename)
            If pZip.GetFileCount() = 0 Then
                oMessage = "Zip file has no file. vbNGYNP57D93!"
                IO.File.Delete(pFilename)
                Return False
            End If
            pInfo = New ProZip(pFilename).GetString("info.json")
            If CStr2(pInfo).Trim = "" Then
                pError = "Info.json does not exist or is empty." ' removed the ref: vb26CQKWA5ZY! tag
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            Try
                pInfoJson = New JsonDictionary()
                pInfoJson.SetJson(pInfo)
                ' 2022_07_16__09_58_31_5048
                If pInfoJson.ContainsKey("AppVersion") Then
                    mIsWebPos = CStr2(pInfoJson.Item("AppVersion")).ToLower().Contains("webpos")
                End If
                If pInfoJson.ContainsKey("MobizAppVersion") Then
                    mMobizAppVersion = pInfoJson.Item("MobizAppVersion")
                End If

            Catch ex As System.Exception
                pError = "Info.json does not contain a valid JSON"
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End Try

            Uuid = pInfoJson("UUID")
            If CStr2(Uuid).Trim = "" Then
                pError = "Info.json does not contain Uuid"
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
            If pInfoJson.ContainsKey("FromFtp") Then
                mFromFtp = CBool2(pInfoJson("FromFtp"))
            End If

            pFilenameOut = AppendBackslash(IO.Path.GetDirectoryName(pFilename)) &
            TimeStamp & "_imported.zip"
            Status = "Start process: " & pFilenameOut
            StatusLastUpdated = Now
            CreateFolder(pFilenameOut)
            pZipOut = New ProZip(pFilenameOut)
            Dim pJson As New JsonDictionary()
            pJson.Add("OrgID", OrgID)
            pJson.Add("UUID", Uuid)
            pJson.Add("DeviceID", DeviceID)
            pJson.Add("AppVersion", "WebPOS")
            pJson.Add("AppVersionName", "WebPOS")
            pJson.Add("AppBuildDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
            pJson.Add("AppInstallDate", Format(#1/1/2000#, cGlobal.FormatDateTimeSql))
            pZipOut.AddString(pJson.ToString(), "info.json")
        End If

        If ImportInvoiceOnly = False Then
            mBusiness.ProData.BeginTransaction()

#If DEBUG Then
            If Now = #1/1/2000# Then
                Throw New ApplicationException("was deadlocked on lock resources with another process and has been chosen as the deadlock victim")
            End If
#End If

            ' additional by janine
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryOrganizationCustomerCreditTerms", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryOrganizationCustomerCreditTerms(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryOrganizationCustomerCreditTerms. ref: vbS8RCSUQZLV!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryOrganization", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryOrganization(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryOrganization. ref: vb669YEPA7W2!")
            End Try

            mBusiness.ProData.CommitTransaction()
            ' additional by janine
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryOrganizationFOWST", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryOrganizationFOWST(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryOrganizationFOWST. ref: vbD4G8VPMW3A!")
            End Try
            ' end of additional
        End If

#If DEBUG Then
        If Now.Date = #4/24/2022# Then
            AddDummyRecords(pFilename)
        End If
#End If

        Try
            mSwAll.Stop()
            InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportInvoice", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
            If ImportInvoice(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                DataImportLog.AddLog("vbUTND9W94VW! Filename: " & pFilename)
                DataImportLog.AddLog("vb2A9K26NW9E! pError: " & pError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                oMessage = JsonDictionary.Serialize("Error", pError)
                Return False
            End If
        Finally
            mSwAll.Start()
            AddLog("ImportInvoice. ref: vb9AMEQCZUDG!")
        End Try

        If ImportInvoiceOnly = False Then
            ' additional by francis sept 04, 2014
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryFOWSTBin", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryFOWSTBin(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryFOWSTBin. ref: vb8QTGS95SR9!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportInvoiceItemsVoided", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportInvoiceItemsVoided(pFilename, pZipOut, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportInvoiceItemsVoided. ref: vb8QTGS95SR9!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportTransCustomerPaymentSRV", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportTransCustomerPaymentSRV(pFilename, pZipOut, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportTransCustomerPaymentSRV. ref: vbEUVPJBHBSU!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportTransZReadCache", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportTransZReadCache(pFilename, pZipOut, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportTransZReadCache. ref: vbEUVPJBHBSU!")
            End Try

            ' additional by francis sept 04, 2014
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryFOWSTBinProduct", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryFOWSTBinProduct(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryFOWSTBinProduct. ref: vbG8PBFNHXE5!")
            End Try

            ' additional by francis march 14, 2017
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryProductsWithoutBarcode", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryProductsWithoutBarcode(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryProductsWithoutBarcode. ref: vbPL5NVLKV4F!")
            End Try

            'mBusiness.ProData.BeginTransaction()
            ' additional by francis sept 03, 2014
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportTransInventoryProductCount", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportTransInventoryProductCount(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.AddLog(pError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportTransInventoryProductCount. ref: vb6CLZKPMRLG!")
            End Try

            'mBusiness.ProData.CommitTransaction()

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportReceiving", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportReceiving(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportReceiving. ref: vbQAJZW2G35T!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportProductBarcode", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportProductBarcode(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportProductBarcode. ref: vbFLUU4URV3W!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportCashHandling", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportCashHandling(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportCashHandling. ref: vb5XN6DZY35Y!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportCashHandlingCashCount", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportCashHandlingCashCount(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportCashHandlingCashCount. ref: vb5XN6DZY35Y!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportExpensesLogDetails", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportExpensesLogDetails(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportExpensesLogDetails. ref: vbGJM8K6CUXM!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportVouchers", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportVouchers(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportVouchers. ref: vbGXLTJR9QUA!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportExpensesCheckInOutDetails", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportExpensesCheckInOutDetails(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportExpensesCheckInOutDetails. ref: vbGXLTJR7SFA!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportExpensesReceiveFunds", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportExpensesReceiveFunds(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportExpensesReceiveFunds. ref: vbZB5HRGN7KJ!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportExpensesNetFundBalance", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportExpensesNetFundBalance(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Catch ex As Exception
                AddLog("ImportExpensesNetFundBalance. ref: vb7JA9AJJI3V!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportVDMLogs", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportVDMLogs(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportVDMLogs. ref: vbGXLTJR8IS7!")
            End Try
            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportBeginningBalance", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportBeginningBalance(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportBeginningBalance. ref: vbCRYLNGEZ8N!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportBomMaker", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportBomMaker(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportBomMaker. ref: vbMUQTRYLDLY!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryDevicePrinterFowst", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryDevicePrinterFowst(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryDevicePrinterFowst. ref: vbYE7MF4CJZS!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryDeviceFowstPrinter", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryDeviceFowstPrinter(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryDeviceFowstPrinter. ref: vb84ZDSCKXGY!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportLibraryDeviceFowst", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportLibraryDeviceFowst(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportLibraryDeviceFowst. ref: vb84ZDSCKXGY!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportActionLogs", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportActionLogs(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportActionLogs. ref: vb84ZDSCKXGZ!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportMemo", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportMemo(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportMemo. ref: vbE9TCBX3PZV!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportMemoSalesReturn", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportMemoSalesReturn(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportMemoSalesReturn. ref: vbW6BC5NUFVD!")
            End Try

            Try
                InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before ImportSalesReturnLedger", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
                mSwAll.Restart()
                If ImportSalesReturnLedger(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportSalesReturnLedger. ref: vbW6BC5NUFVD!")
            End Try

            Try
                If ImportTenantMonitoringLogs(pFilename, pOut, pZipOut, OrgID, Uuid, pError) = False Then
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    oMessage = JsonDictionary.Serialize("Error", pError)
                    Return False
                End If
            Finally
                AddLog("ImportTenantMonitoringLogs. ref: vbW6BC5NUFVE!")
            End Try

        End If

        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Before vbDKGQEEEAVQ", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
        mSwAll.Restart()
        ' todo: uncomment and debug this
        ' vbQKCASMLJBK!
        pDatabaseName = mBusiness.ProData.GetCurrentDatabase
        'pFranchiseeDataExporter = New FranchiseeDataExporter(mBusiness.ProData.Clone) ' old
        'pFranchiseeDataExporter = New FranchiseeDataExporter(mBusiness.ProData) ' new
        TimedThread(
            Sub()
                'Try
                '    pFranchiseeDataExporter.CopyRecords()
                '    If pMustRecacheInvoice Then
                '        mBusiness.TransInvoice.CacheInvoicesWithCredit()
                '    End If
                'Catch ex As Exception
                '    AppendToLog("vbHAMRWRBVME! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "FranchiseeDataExporter")
                'End Try
            End Sub)

        ' 2018_09_04__10_00_45_5180
        ' BEGIN trial: DONT put this inside thread
        'Try
        '    pFranchiseeDataExporter.CopyRecords(OrgID)
        '    AddLog("pFranchiseeDataExporter.CopyRecords(). ref: vbPEBMC8LCWE!")
        '    If pMustRecacheInvoice Then
        'mBusiness.TransInvoice.CacheInvoicesWithCredit() ' commented for testing 2021_08_27__09_18_23_9608
        'AddLog("mBusiness.TransInvoice.CacheInvoicesWithCredit(). ref: vb3UPVH3PUYA!") ' commented for testing 2021_08_27__09_18_23_9608
        '    End If
        'Catch ex As Exception
        '    AppendToLog("vbHAMRWRBVME! " & ex.Message & vbCrLf & GetStackTrace(, ex), , "FranchiseeDataExporter")
        'End Try
        ' END trial: DONT put this inside thread

        ' todo: uncomment and debug this
        ' vbQKCASMLJBK!

        ' BEGIN commented as of 2020_07_27__09_15_04_6053
        'Try
        '    Call New RowCountChecksum(OrgID, DeviceID, pFilename).Process()
        'Catch ex As Exception
        '    AppendToLog("vbH9JDY7BW98! Error in RowCountChecksum: " & ex.Message)
        'End Try
        'AddLog("Call New RowCountChecksum(OrgID, DeviceID, pFilename).Process(). ref: vbQK295DQMSR!")
        ' END commented as of 2020_07_27__09_15_04_6053

        If pZipOut.GetFileCount > 0 Then
            pOut.Add("Filename", InputFilename)
        End If
        pOut.Add("Result", "ok")
        DataImportLog.Save(DataImportLog.eResultType.Success)
        oMessage = pOut.Serialize
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="End Thread vbU3LJL8V3Q9", vTotalDuration:=mSwAll.ElapsedMilliseconds, vRecordCount:=0)
        mSwAll.Restart()

        Return True

    End Function

    Private Sub ImportFrom2ndInstance()

        ImportInvoicesFrom2ndInstance()
        ImportCustomerPaymentsFrom2ndInstance()
        ImportSalesReturnsFrom2ndInstance()
        ImportCacheInvoicesFrom2ndInstance()
        ImportCacheInventoryMovementFrom2ndInstance()

    End Sub

    Private Sub ImportInvoicesFrom2ndInstance()

        Dim pSql = "SELECT TOP " & mFastFlushingBatchSize & " * " & vbCrLf &
            " INTO #tempTransInvoice" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        mBusinessSource.ProData.BeginTransaction()
        mBusinessSource.ProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tempTransInvoice"
        Dim pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            mBusiness.ProData.BeginTransaction()
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransInvoice"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If

            pSql = "SELECT TransInvoiceItems.* " & vbCrLf &
                " FROM TransInvoiceItems" & vbCrLf &
                " WHERE TransInvoiceItems.InvoiceID IN (SELECT ID from #tempTransInvoice)" +
                " ORDER BY TransInvoiceItems.LineNumber"
            Dim pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceItems"
                pIU.Remove("COGS") ' computed
                pIU.Remove("DiscountTax") ' computed
                pIU.Remove("DiscountedUnitPrice") ' computed
                pIU.Remove("GrossSalesNetOfVATExemptions") ' computed
                pIU.Remove("HeaderArea") ' derived column
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceItemSerials.* " & vbCrLf &
                " FROM TransInvoiceItemSerials" & vbCrLf &
                " WHERE TransInvoiceItemSerials.InvoiceID IN (SELECT ID from #tempTransInvoice)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceItemSerials"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceDiscount.* " & vbCrLf &
                " FROM TransInvoiceDiscount" & vbCrLf &
                " WHERE TransInvoiceDiscount.InvoiceID IN (SELECT ID from #tempTransInvoice)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceDiscount"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceSeniorCitizenPWDCustomer.* " & vbCrLf &
                " FROM TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
                " WHERE TransInvoiceSeniorCitizenPWDCustomer.InvoiceID IN (SELECT ID from #tempTransInvoice)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceSeniorCitizenPWDCustomer"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceCharges.* " & vbCrLf &
                " FROM TransInvoiceCharges" & vbCrLf &
                " WHERE TransInvoiceCharges.InvoiceID IN (SELECT ID from #tempTransInvoice)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceCharges"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceItemsVoided.* " & vbCrLf &
                " FROM TransInvoiceItemsVoided" & vbCrLf &
                " WHERE TransInvoiceItemsVoided.InvoiceID IN (SELECT ID from #tempTransInvoice)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceItemsVoided"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceItemsModifier.* " & vbCrLf &
                " FROM TransInvoiceItemsModifier" & vbCrLf &
                " WHERE TransInvoiceItemsModifier.InvoiceItemID IN " & vbCrLf &
                " (SELECT ID" & vbCrLf &
                "   FROM TransInvoiceItems" & vbCrLf &
                "   WHERE InvoiceID IN (SELECT ID from #tempTransInvoice)" & vbCrLf &
                " )" & vbCrLf
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceItemsModifier"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceItemSalesReps.* " & vbCrLf &
                " FROM TransInvoiceItemSalesReps" & vbCrLf &
                " WHERE TransInvoiceItemSalesReps.InvoiceItemID IN " & vbCrLf &
                " (SELECT ID" & vbCrLf &
                "   FROM TransInvoiceItems" & vbCrLf &
                "   WHERE InvoiceID IN (SELECT ID from #tempTransInvoice)" & vbCrLf &
                " )" & vbCrLf
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceItemSalesReps"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransInvoiceItemsRebates.* " & vbCrLf &
                " FROM TransInvoiceItemsRebates" & vbCrLf &
                " WHERE TransInvoiceItemsRebates.ModifierGroupID IN " & vbCrLf &
                " (SELECT ModifierGroupID" & vbCrLf &
                "   FROM TransInvoiceItems" & vbCrLf &
                "   WHERE InvoiceID IN (SELECT ID from #tempTransInvoice)" & vbCrLf &
                " )" & vbCrLf
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransInvoiceItemsRebates"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pIU = New InsertUpdate("TransInvoice")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
            mBusiness.ProData.CommitTransaction()
        Next
        mBusinessSource.ProData.CommitTransaction()

    End Sub

    Private Sub ImportCustomerPaymentsFrom2ndInstance()

        Dim pSql = "SELECT TOP " & mFastFlushingBatchSize & " * " & vbCrLf &
            " INTO #tempTransCustomerPayment" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        mBusinessSource.ProData.BeginTransaction()
        mBusinessSource.ProData.ExecuteSQL(pSql)

        pSql = "SELECT * FROM #tempTransCustomerPayment"
        Dim pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            mBusiness.ProData.BeginTransaction()
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransCustomerPayment"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If

            pSql = "SELECT TransCustomerPaymentInvoices.* " & vbCrLf &
                " FROM TransCustomerPaymentInvoices" & vbCrLf &
                " WHERE TransCustomerPaymentInvoices.CustomerPaymentID IN (SELECT ID from #tempTransCustomerPayment)"
            Dim pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransCustomerPaymentInvoices"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransCustomerPaymentDebitCard.* " & vbCrLf &
                " FROM TransCustomerPaymentDebitCard" & vbCrLf &
                " WHERE TransCustomerPaymentDebitCard.CustomerPaymentID IN (SELECT ID from #tempTransCustomerPayment)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransCustomerPaymentDebitCard"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransCustomerPaymentCreditCard.* " & vbCrLf &
                " FROM TransCustomerPaymentCreditCard" & vbCrLf &
                " WHERE TransCustomerPaymentCreditCard.CustomerPaymentID IN (SELECT ID from #tempTransCustomerPayment)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransCustomerPaymentCreditCard"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransCustomerPaymentChecks.* " & vbCrLf &
                " FROM TransCustomerPaymentChecks" & vbCrLf &
                " WHERE TransCustomerPaymentChecks.CustomerPaymentID IN (SELECT ID from #tempTransCustomerPayment)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransCustomerPaymentChecks"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pSql = "SELECT TransCustomerPaymentOther.* " & vbCrLf &
                " FROM TransCustomerPaymentOther" & vbCrLf &
                " WHERE TransCustomerPaymentOther.CustomerPaymentID IN (SELECT ID from #tempTransCustomerPayment)"
            pDtItems = mBusinessSource.ProData.Execute(pSql)
            For Each pDritem As DataRow In pDtItems.Rows
                pIU = Business.DataRowToInsertUpdate(pDritem)
                pIU.TableName = "TransCustomerPaymentOther"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pDritem("ID")))
                pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
            Next pDritem

            pIU = New InsertUpdate("TransCustomerPayment")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
            mBusiness.ProData.CommitTransaction()
        Next
        mBusinessSource.ProData.CommitTransaction()

    End Sub

    Private Sub ImportSalesReturnsFrom2ndInstance()

        Dim pSql = "SELECT TOP " & mFastFlushingBatchSize & " * " & vbCrLf &
            " FROM TransMemo" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        Dim pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransMemo"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If

            pIU = New InsertUpdate("TransMemo")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next

        pSql = "SELECT TOP " & mFastFlushingBatchSize & " * " & vbCrLf &
            " FROM TransMemoSalesReturn" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransMemoSalesReturn"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If

            pIU = New InsertUpdate("TransMemoSalesReturn")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next

        pSql = "SELECT TOP " & mFastFlushingBatchSize & " * " & vbCrLf &
            " FROM TransSalesReturnLedger" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "TransSalesReturnLedger"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If

            pIU = New InsertUpdate("TransSalesReturnLedger")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next

    End Sub

    Private Sub ImportCacheInvoicesFrom2ndInstance()

        Dim pSql = "SELECT TOP 10 * FROM CacheInvoices" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY LastUpdated"
        Dim pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "CacheInvoices"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If
            pIU = New InsertUpdate("CacheInvoices")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next

    End Sub

    Private Sub ImportCacheInventoryMovementFrom2ndInstance()

        Dim pSql = "SELECT TOP 10 * FROM CacheInventoryMovement" & vbCrLf &
            " WHERE ConsolidationDate IS NULL" & vbCrLf &
            " ORDER BY TransactionDate"
        Dim pDT = mBusinessSource.ProData.Execute(pSql)
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = Business.DataRowToInsertUpdate(pDR)
            pIU.TableName = "CacheInventoryMovement"
            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                " AND FowstID = " & ReadyString(pDR("FowstID")) & vbCrLf &
                " AND CONVERT(date, TransactionDate) = " & ReadyString(Format(pDR("TransactionDate"))))
            Dim pAffectedRecords = mBusiness.ProData.ExecuteSQL(pSql)
            If pAffectedRecords = 0 Then
                pSql = pIU.GetInsert()
                mBusiness.ProData.ExecuteSQL(pSql)
            End If
            pIU = New InsertUpdate("CacheInventoryMovement")
            pIU("ConsolidationDate") = "GETDATE()"
            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                " AND FowstID = " & ReadyString(pDR("FowstID")) & vbCrLf &
                " AND CONVERT(date, TransactionDate) = " & ReadyString(Format(pDR("TransactionDate"))))
            pAffectedRecords = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
        Next

    End Sub

    Private Sub ExtractSignatures(vZipFilename As String)

        Dim pZip As C1ZipFile
        Dim pFolder As String
        Dim pFilename As String

        Try
            pZip = New C1ZipFile(vZipFilename)
            For Each pEntry As C1ZipEntry In pZip.Entries
                If pEntry.Attributes And IO.FileAttributes.Directory Then
                    Continue For
                End If
                pFolder = IO.Path.GetDirectoryName(pEntry.FileName)
                If pFolder.ToLower() = "signatures" Then
                    pFilename = Common.ServerPath & "temp\Uploads\SalesOrderSignature\" & OrgID & "\" & IO.Path.GetFileName(pEntry.FileName)
                    CreateFolder(pFilename)
                    pEntry.Extract(pFilename)
                End If
            Next pEntry
            pZip.Close()
        Catch ex As Exception
            AppendToLog("vbTUCNEFCYS9! Error in ExtractSignatures. Filename: " & vZipFilename & "; Error: " & ex.Message)
        End Try

    End Sub

    Private Function ImportInvoice(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Const pMinimumDate = #2025/08/01# ' for 3rd party org only

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut, pDtOutItems As DataTable
        Dim pDrOut As DataRow
        Dim pNum As Integer
        Dim pDtItems As DataTable
        Dim pCreator As TransInvoice.Creator
        Dim mItemInitialized As Boolean
        Dim pDrInvoice As DataRow
        Dim pDataImportLogAlreadyImported As DataImportLog
        Dim pMaxSubNumber As Integer?
        Dim pExistingInvoiceCount As Integer ' applicable only to free user
        Dim pStr As String
        Dim pIsPo As Boolean
        Dim pInvoiceCount As Integer
        Dim pItemCount As Integer
        Dim pItemModifiersStopWatch As New Stopwatch
        Dim pInventoryStopWatch As New Stopwatch
        Dim pSalesRepStopWatch As New Stopwatch
        Dim pItemRebatesStopWatch As New Stopwatch
        Dim pItemSerialsStopWatch As New Stopwatch
        Dim pItemDiscountStopWatch As New Stopwatch
        Dim pSeniorPwdStopWatch As New Stopwatch
        Dim pInvoiceChargesStopWatch As New Stopwatch
        Dim pItemsVoidedStopWatch As New Stopwatch
        Dim pEmailStopWatch As New Stopwatch
        Dim pEmailCount As Integer
        Dim pSavedInvoiceIDs As New List(Of String)
        Dim pImportedInvoices As String
        Dim pImportedPaymentInvoices As String

        ' customer payments
        Dim pDtCustomerPayments As DataTable
        Dim pBinderCustomerPayments As DictionaryBinder
        Dim pDtOutCustomerPayments As DataTable

        Dim pDtCustomerPaymentInvoices As DataTable
        Dim pBinderCustomerPaymentInvoices As DictionaryBinder

        Dim pDtCustomerPaymentDebitCards As DataTable
        Dim pBinderCustomerPaymentDebitCards As DictionaryBinder

        Dim pDtCustomerPaymentCreditCards As DataTable
        Dim pBinderCustomerPaymentCreditCards As DictionaryBinder

        Dim pDtCustomerPaymentChecks As DataTable
        Dim pBinderCustomerPaymentChecks As DictionaryBinder

        Dim pDtCustomerPaymentOther As DataTable
        Dim pBinderCustomerPaymentOther As DictionaryBinder

        Dim lDeviceID As String
        Dim lFOWSTID As String
        Dim lInvoiceDate As String
        Dim pSW2 = Stopwatch.StartNew() ' for TransImportBenchmark2
        Dim pHeaderDuration As Integer
        Dim pItemsDuration As Integer
        Dim pDtInvoice As DataTable ' for fast flushing only

        oError = ""

        If EnableLogs Then AppendToLog("vbETEZZZNJ73 ImportInvoice " & InputFilename, , mLogFile)
        If mFreeUser Then
            ' pExistingInvoiceCount = mBusiness.TransInvoice.GetCountByMonth(ParseDate(pLine("InvoiceDate"))) ' use this if you want to determine the month by invoice date
            pExistingInvoiceCount = mBusiness.TransInvoice.GetCountByMonth()
            If TransInvoice.FreeUserMaxInvoiceCount <= pExistingInvoiceCount Then
                mBusiness.ProData.RollbackTransaction(True, True)
                oError = "As a free user, you have exceeded your maximum allowed number of invoices of " & TransInvoice.FreeUserMaxInvoiceCount & " for the month of " & Format(Now, "MMMM yyyy")
                Return False
            End If
        End If

        If DeviceID = modClient.SportsTownOrgID Then
            AppendToLog("vb2C3KF8UJFJ! DeviceID=" & DeviceID, , "ImportDataClass2")
        End If

        DataImportLog.AddLog("Import from TransInvoice")

        If ReadyGuid(OrgID) = ReadyGuid(modClient.PotatoCornerFranchise2OrgID) Then
            pCsvReader = New CsvReader(vInputZipFilename, "TransInvoice")
            Do
                pLine = pCsvReader.ReadLine()
                If pLine Is Nothing Then
                    Exit Do
                End If
                Dim pInvoiceDate = ParseDate(pLine("InvoiceDate"))
                If pInvoiceDate IsNot Nothing Then
                    If pInvoiceDate.Value < pMinimumDate Then
                        oError = "This zip file will not be processed because it contains an invoice with an invoice date (" & pInvoiceDate.Value.ToString(cGlobal.FormatDateTime) & ") that is earlier than the minimum date (" & pMinimumDate.ToString(cGlobal.FormatDate) & "). ref: vbAK9CTW7ZEK!"
                        Return False
                    End If
                End If
            Loop
        End If

        If mConsolidationMode Then
            Dim pInvoiceIds As String
            pSql = "SELECT TOP " & mFastFlushingBatchSize & " * " & vbCrLf &
                " FROM TransInvoice" & vbCrLf &
                " WHERE ConsolidationDate IS NULL" & vbCrLf &
                " ORDER BY LastUpdated"
            pDtInvoice = mBusinessSource.ProData.Execute(pSql)

            If pDtInvoice.Rows.Count > 0 Then
                pInvoiceIds = CStr2(ExtractFieldValues(pDtInvoice, "ID", Integer.MaxValue)(0)("ID"))

                pDtItems = mBusinessSource.ProData.Execute("SELECT TransInvoiceItems.* " +
                    " FROM TransInvoiceItems" +
                    " WHERE TransInvoiceItems.InvoiceID IN (" + pInvoiceIds + ")" +
                    " ORDER BY TransInvoiceItems.LineNumber")
                mItemInitialized = True
            End If

            pSql = "SELECT * FROM TransCustomerPayment" +
                " WHERE ConsolidationDate IS NULL" & vbCrLf &
                " ORDER BY LastUpdated"
            pDtCustomerPayments = mBusinessSource.ProData.Execute(pSql)
            CreateCustomerPaymentsDataTable(Nothing, pBinderCustomerPayments, oError, Nothing)
            If pDtCustomerPayments.Rows.Count > 0 Then
                Dim pCustomerPaymentIds = CStr2(ExtractFieldValues(pDtCustomerPayments, "ID", Integer.MaxValue)(0)("ID"))
                pDtCustomerPaymentInvoices = mBusinessSource.ProData.Execute("SELECT TransCustomerPaymentInvoices.* " +
                        " FROM TransCustomerPaymentInvoices" +
                        " WHERE TransCustomerPaymentInvoices.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
                CreateCustomerPaymentInvoicesDataTable(Nothing, pBinderCustomerPaymentInvoices, oError)

                pDtCustomerPaymentDebitCards = mBusinessSource.ProData.Execute("SELECT TransCustomerPaymentDebitCard.*" + vbCrLf +
                    " FROM TransCustomerPaymentDebitCard" + vbCrLf +
                    " WHERE TransCustomerPaymentDebitCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
                CreateCustomerPaymentDebitCardDataTable(Nothing, pBinderCustomerPaymentDebitCards, oError)

                pDtCustomerPaymentCreditCards = mBusinessSource.ProData.Execute("SELECT TransCustomerPaymentCreditCard.*" + vbCrLf +
                    " FROM TransCustomerPaymentCreditCard" + vbCrLf +
                    " WHERE TransCustomerPaymentCreditCard.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
                CreateCustomerPaymentCreditCardDataTable(Nothing, pBinderCustomerPaymentCreditCards, oError)

                pDtCustomerPaymentChecks = mBusinessSource.ProData.Execute("SELECT TransCustomerPaymentChecks.*" + vbCrLf +
                    " FROM TransCustomerPaymentChecks" + vbCrLf +
                    " WHERE TransCustomerPaymentChecks.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
                CreateCustomerPaymentCheckDataTable(Nothing, pBinderCustomerPaymentChecks, oError)

                pDtCustomerPaymentOther = mBusinessSource.ProData.Execute("SELECT TransCustomerPaymentOther.*" + vbCrLf +
                    " FROM TransCustomerPaymentOther" + vbCrLf +
                    " WHERE TransCustomerPaymentOther.CustomerPaymentID IN (" + pCustomerPaymentIds + ")")
                CreateCustomerPaymentOtherDataTable(Nothing, pBinderCustomerPaymentOther, oError)
            End If
        Else
            pDtCustomerPayments = CreateCustomerPaymentsDataTable(vInputZipFilename, pBinderCustomerPayments, oError, pDtInvoice)
            If pDtCustomerPayments Is Nothing Then
                oError &= ". Zip=" & vInputZipFilename
                Return False
            End If

            pDtCustomerPaymentInvoices = CreateCustomerPaymentInvoicesDataTable(vInputZipFilename, pBinderCustomerPaymentInvoices, oError)
            If pDtCustomerPaymentInvoices Is Nothing Then
                Return False
            End If

            pDtCustomerPaymentDebitCards = CreateCustomerPaymentDebitCardDataTable(vInputZipFilename, pBinderCustomerPaymentDebitCards, oError)
            If pDtCustomerPaymentDebitCards Is Nothing Then
                Return False
            End If

            pDtCustomerPaymentCreditCards = CreateCustomerPaymentCreditCardDataTable(vInputZipFilename, pBinderCustomerPaymentCreditCards, oError)
            If pDtCustomerPaymentCreditCards Is Nothing Then
                Return False
            End If

            pDtCustomerPaymentChecks = CreateCustomerPaymentCheckDataTable(vInputZipFilename, pBinderCustomerPaymentChecks, oError)
            If pDtCustomerPaymentChecks Is Nothing Then
                Return False
            End If

            pDtCustomerPaymentOther = CreateCustomerPaymentOtherDataTable(vInputZipFilename, pBinderCustomerPaymentOther, oError)
            If pDtCustomerPaymentOther Is Nothing Then
                Return False
            End If
        End If

        DataImportLog.AddLog("Import from TransInvoice")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoice")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceNumber", True, SqlDbType.Int)
            .Add("InvoiceDate", True, SqlDbType.DateTime)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("BillingAddressID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("DeliveryAddressID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
            .Add("UserID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("RefNum", False, SqlDbType.VarChar)
            .Add("ReasonForReconstructingData", False, SqlDbType.VarChar)
            .Add("Remarks", False, SqlDbType.VarChar)
            .Add("DeliveryDate", False, SqlDbType.VarChar) ' ideally should be SqlDbType.DateTime
            .Add("IsCancelled", False, SqlDbType.Bit)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("SalesOrderNo", False, SqlDbType.Int)
            .Add("SalesOrderDate", False, SqlDbType.Date)
            .Add("SalesOrderUserID", False, SqlDbType.UniqueIdentifier)
            .Add("CustomerTableNo", False, SqlDbType.VarChar)
            .Add("IsForGovReport", False, SqlDbType.Bit)
            .Add("TotalAmount", False, SqlDbType.Money)
            .Add("IsTestOnly", False, SqlDbType.Bit) ' added by janine. for the self test
            .Add("PurchaseOrderNo", False, SqlDbType.VarChar) ' added by janine. for sales order taker aug 6
            .Add("PurchaseOrderDate", False, SqlDbType.Date) ' added by janine. for sales order taker aug 6
            .Add("PurchaserName", False, SqlDbType.VarChar) ' added by janine. for sales order taker aug 6
            .Add("SalesOrderSignatory", False, SqlDbType.VarChar) ' added by janine. for sales order taker signature oct 8
            .Add("DeliveryMethod", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("OrderMethod", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("CustomerDemographicGender", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("CustomerDemographicAge", False, SqlDbType.VarChar) ' added by janine. for customer demographic oct 24
            .Add("FranchiseeOrgID", False, SqlDbType.VarChar) ' added by francis. Nov 3
            .Add("SupplierID", False, SqlDbType.UniqueIdentifier)
            .Add("SupplierFowstID", False, SqlDbType.VarChar) ' temp solution 2016_08_18__16_07_52_6115
            .Add("InvoiceTypeCode", False, SqlDbType.VarChar)
            .Add("ReceivingID", False, SqlDbType.UniqueIdentifier)
            .Add("SupplierBinID", False, SqlDbType.UniqueIdentifier)
            .Add("CustomerFowstID", False, SqlDbType.UniqueIdentifier)
            .Add("CustomerBinID", False, SqlDbType.UniqueIdentifier)

            .Add("VAT", False, SqlDbType.Money)
            .Add("Vatable", False, SqlDbType.Money)
            .Add("VATExempt", False, SqlDbType.Money)
            .Add("ForegoneVAT", False, SqlDbType.Money)
            .Add("PhNatTeamDiscount", False, SqlDbType.Money)
            .Add("SoloParentDiscount", False, SqlDbType.Money)
            .Add("SCDiscount", False, SqlDbType.Money)
            .Add("SCDiscountExempt", False, SqlDbType.Money)
            .Add("TotalNonSCDiscount", False, SqlDbType.Money)
            .Add("GrossSalesToRegDiner", False, SqlDbType.Money)
            .Add("GrossSalesToSC", False, SqlDbType.Money)
            .Add("WeightedSCDiscountRate", False, SqlDbType.Money)
            .Add("AmountDueBeforeCharges", False, SqlDbType.Money)
            .Add("AmountDueNetofCharges", False, SqlDbType.Money)
            .Add("PrintCount", False, SqlDbType.Int)
            .Add("ReprintedDate", False, SqlDbType.DateTime)
            .Add("IsPrinted", False, SqlDbType.Bit)
            .Add("DepositAmount", False, SqlDbType.Money)
            .Add("IsNoneVATSale ", False, SqlDbType.Bit)
            .Add("TransType ", False, SqlDbType.VarChar)
            .Add("DeviceFowstTransactionNumber", False, SqlDbType.Int)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")
        pDtOut.Columns.Add("LastUpdated", GetType(Date))
        pDtOut.Columns.Add("InvoiceNumber", GetType(Integer))

        Dim dtAdjustmentItems As Date
        Dim IsHasPossibleReconstruction As Boolean
        Dim pSW = Stopwatch.StartNew()
        Dim pSwBatch = Stopwatch.StartNew()
        Dim pRowIndex As Integer = -1

        Try
            pImportedInvoices = FileText(GetImportedInvoicesFilename())
        Catch ex As Exception
            Debug.WriteLine("just ignore this error.")
        End Try
        pImportedInvoices = CStr2(pImportedInvoices)

        Try
            pImportedPaymentInvoices = FileText(GetImportedPaymentInvoicesFilename())
        Catch ex As Exception
            Debug.WriteLine("just ignore this error.")
        End Try
        pImportedPaymentInvoices = CStr2(pImportedPaymentInvoices)

        Do
            pSW2.Start()
            pRowIndex += 1
            If pRowIndex > 25 Then
                If ReadyGuid(OrgID) = ReadyGuid(modClient.CincoOrgID) _
                    OrElse ReadyGuid(OrgID) = ReadyGuid(modClient.PotatoCornerFranchiseOrgID) Then

                    Exit Do
                End If
            End If
            IsHasPossibleReconstruction = False
            mBusiness.ProData.RollbackTransaction(True, True)

            If False Then
                Throw New Exception("Test exception vbWS8E53NK2C")
            End If

            If pDtInvoice Is Nothing Then
                pLine = pCsvReader.ReadLine
            Else
                If pRowIndex > pDtInvoice.Rows.Count - 1 Then
                    Exit Do
                End If
                pLine = pDtInvoice.Rows(pRowIndex).Table.Columns _
                    .Cast(Of DataColumn) _
                    .ToDictionary(
                        Function(x) x.ColumnName,
                        Function(x) CStr2(pDtInvoice.Rows(pRowIndex)(x))
                    )
            End If
            If pLine Is Nothing Then Exit Do

            'additional by janine sept 8 2014 5pm for sales order taker
            Dim posmode As String
            If pLine.ContainsKey("SalesOrderNo") AndAlso CStr2(pLine("SalesOrderNo")).Trim <> "" Then
                posmode = "so"
            Else
                posmode = "pos"
            End If
            ''''''''''''''''''''

            If pLine.ContainsKey("BillingAddressID") = False _
                OrElse IsGuid(pLine("BillingAddressID")) = False Then

                pLine("BillingAddressID") = mBusiness.SystemDefaults.GetSystemDefault("POS Billing Address ID")
            End If
            If pLine.ContainsKey("DeliveryAddressID") = False _
                OrElse IsGuid(pLine("DeliveryAddressID")) = False Then

                pLine("DeliveryAddressID") = mBusiness.SystemDefaults.GetSystemDefault("POS Delivery Address ID")
            End If

            ' commented 2022_03_04__14_55_57_3362
            'If mBusiness.LibraryPeople.Validate(
            '    vPeopleID:=pLine("UserID"),
            '    vOrgID:=vOrgID,
            '    vFowstID:=pLine("FowstID"),
            '    vPosMode:=posmode,
            '    vDeviceID:=pLine("DeviceID"),
            '    vVerifyUuid:=True,
            '    vDeviceUuid:=vUuid,
            '    oError:=oError,
            '    vDontVerifyPeopleFowst:=True) = False Then

            '    Exit Do
            'End If

            If mFowstID Is Nothing Then
                mFowstID = pLine("FowstID")
            End If
            If mItemInitialized = False Then
                mItemInitialized = True
                pDtItems = CreateInvoiceItemsDataTable(vInputZipFilename, oError)
                If pDtItems Is Nothing Then
                    Return False
                End If
            End If
            If EnableLogs Then AppendToLog("vbNWBHR83QPC! pDtItems.Rows.Count:" & pDtItems.Rows.Count, , mLogFile)

            If DeviceID = modClient.SportsTownOrgID Then
                AppendToLog("vbWX5D2U6TGW! InvoiceID " & pLine("ID"), , "ImportDataClass2")
            End If

            If IsGuid(pLine("ID")) Then
                If pImportedInvoices.Contains(ReadyGuid(pLine("ID"))) Then
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("InvoiceNumber") = pLine("InvoiceNumber")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)
                    Continue Do
                End If
                pSql = "SELECT TransInvoice.InvoiceNumber" & vbCrLf &
                    ",TransInvoice.FowstID" & vbCrLf &
                    " FROM TransInvoice" &
                    " WHERE TransInvoice.ID = " & ReadyString(pLine("ID"))
                'pDrInvoice = mBusiness.TransInvoice.ById(pLine("ID"))
                pDrInvoice = mBusiness.ProData.ExecuteRow(pSql)
                If pDrInvoice IsNot Nothing _
                    AndAlso CDec2(pDrInvoice("InvoiceNumber")) <> 0 _
                    AndAlso AllowReimportNonZeroInvoice = False Then

                    If DeviceID = modClient.SportsTownOrgID Then
                        AppendToLog("vbEM9YUVSB5W! already imported " & pLine("ID"), , "ImportDataClass2")
                    End If

                    If pDataImportLogAlreadyImported Is Nothing Then
                        pDataImportLogAlreadyImported = New DataImportLog
                        pDataImportLogAlreadyImported.AddLog("InputZipFilename: " & vInputZipFilename)
                        pDataImportLogAlreadyImported.AddLog("OrgID: " & OrgID)
                    End If
                    pDataImportLogAlreadyImported.AddLog("InvoiceID: " & pLine("ID"))
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("InvoiceNumber") = pLine("InvoiceNumber")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)
                    If pLine.ContainsKey("PrintCount") Then
                        If CInt2(pLine("PrintCount")) > 0 Then
                            Dim pIuPrintCount = New InsertUpdate("TransInvoice")
                            pIuPrintCount("PrintCount") = CInt2(pLine("PrintCount"))
                            pSql = pIuPrintCount.GetUpdate("ID = " & ReadyString(pLine("ID")))
                            pSql = mBusiness.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                        End If
                    End If
                    If mConsolidationMode Then
                        Dim pIuConsolidate = New InsertUpdate("TransInvoice")
                        pIuConsolidate("ConsolidationDate") = "GETDATE()"
                        pSql = pIuConsolidate.GetUpdate("ID = " & ReadyString(pLine("ID")))
                        pSql = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                    End If
                    AppendToImportedInvoicesFile(pLine("ID"))
                    Continue Do
                End If

                If mFromFtp Then
                    pDrInvoice = mBusiness.TransInvoice.GetByInvoiceNumFowstIDDeviceSerial(pLine("InvoiceNumber"), pLine("FowstID"), Uuid)
                    If pDrInvoice IsNot Nothing Then
                        pDrOut = pDtOut.NewRow
                        pDrOut("ID") = pLine("ID")
                        pDrOut("InvoiceNumber") = pLine("InvoiceNumber")
                        pDrOut("LastUpdated") = pLine("LastUpdated")
                        pDtOut.Rows.Add(pDrOut)
                        AppendToImportedInvoicesFile(pLine("ID"))
                        Continue Do
                    End If
                End If

                'mBusiness.TransInvoice.VerifyCounter(pLine("DeviceID"), pLine("FowstID"), pNum)
                'If CDec2(pLine("InvoiceNumber")) <= pNum Then
                '    DataImportLog.AddLog("Existing max invoice number/counter is: " & pNum & " but the invoice number to be imported is " & pLine("InvoiceNumber") & ". Imported number will become " & pNum + 1 & ". ID: " & pLine("ID") & ". Ref vbXAJ5YY4ELK")
                '    pLine("InvoiceNumber") = pNum + 1 ' TODO: apply business logic here
                '    Stop2()
                'ElseIf CDec2(pLine("InvoiceNumber")) <> pNum + 1 Then
                '    DataImportLog.AddLog("The invoice number to be imported is " & pLine("InvoiceNumber") & ". Imported number must be " & pNum + 1 & ". ID: " & pLine("ID") & ". Ref vbPA8DFTRKQX")
                '    Stop2()
                'End If
            Else
                Continue Do
            End If

            If CDec2(pLine("InvoiceNumber")) < 0 Then
                pLine("InvoiceNumber") = 0 ' prevent negative number
            End If
            If pLine.ContainsKey("DeliveryDate") Then
                If IsDate(pLine("DeliveryDate")) = False Then
                    pLine.Remove("DeliveryDate")
                End If
            End If
            '' special case ni xa
            'If pLine("LastUpdated").Trim.ToLower = "Thu Mar 20 2014 18:08:43 GMT+0800 (PHT)".ToLower Then
            '    pLine("LastUpdated") = "2014-03-20 18:08:43"
            'End If
            'If pLine("LastUpdated").Trim.ToLower = "Fri Mar 21 2014 16:56:47 GMT+0800 (PHT)".ToLower Then
            '    pLine("LastUpdated") = "2014-03-21 16:56:47"
            'End If
            'If pLine("LastUpdated").Trim.ToLower = "Sun Mar 23 2014 17:22:33 GMT+0800 (PHT)".ToLower Then
            '    pLine("LastUpdated") = "2014-03-23 17:22:33"
            'End If
            If ImportInvoiceOnly Then
                If InvoiceIDToImport Is Nothing Then
                    Throw New NullReferenceException("vbJAXZHHF5ET! InvoiceIDToImport must not be null when ImportInvoiceOnly = true")
                End If
                If InvoiceIDToImport.Contains(ReadyGuid(pLine("ID"))) = False Then
                    Continue Do
                End If
            End If

            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Exit Do
            End If
            If mAllowImportDrafts = False _
                AndAlso CDec2(pLine("InvoiceNumber")) <= 0 _
                AndAlso (
                    pLine.ContainsKey("SalesOrderNo") = False _
                    OrElse CDec2(pLine("SalesOrderNo")) <= 0
                ) _
                AndAlso (
                    pLine.ContainsKey("PurchaseOrderNo") = False _
                    OrElse CDec2(pLine("PurchaseOrderNo")) <= 0
                ) Then

                DataImportLog.AddLog("ID " & pLine("ID") & " has an invoice number of " & CDec2(pLine("InvoiceNumber")) & " and will not be imported. This row will be tagged as imported on the device. Ref VZ8KTV7LAU")
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
                If mConsolidationMode Then
                    Dim pIuConsolidate = New InsertUpdate("TransInvoice")
                    pIuConsolidate("ConsolidationDate") = "GETDATE()"
                    pSql = pIuConsolidate.GetUpdate("ID = " & ReadyString(pLine("ID")))
                    pSql = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                End If
                Continue Do
            End If

            pCreator = New TransInvoice.Creator(mBusiness.ProData, mBusiness.TransInvoice)
            pCreator.ID = pLine("ID")
            pCreator.InvoiceNumber = pLine("InvoiceNumber")

            If ParseDate(pLine("InvoiceDate")) Is Nothing Then
                Dim transformDate As String = DateTime.ParseExact(pLine("InvoiceDate"),
                                    "d/M/yyyy h:mm:ss tt",
                                     System.Globalization.CultureInfo.InvariantCulture).ToString("M/d/yyyy h:mm:ss tt")
                pCreator.InvoiceDate = ParseDate(transformDate)
            Else
                pCreator.InvoiceDate = ParseDate(pLine("InvoiceDate"))
            End If

            pCreator.FowstID = pLine("FowstID")
            pCreator.DeviceID = pLine("DeviceID")

            ' 2026-04-06
            'If pLine.ContainsKey("DeviceFowstTransactionNumber") Then
            '    pCreator.DeviceFowstTransactionNumber = pLine("DeviceFowstTransactionNumber")
            'End If
            Dim pDrDeviceFowst = mBusiness.LibraryDeviceFowst.ByDeviceFowst(pLine("DeviceID"), pLine("FowstID"))
            If pDrDeviceFowst IsNot Nothing Then
                'Dim pDeviceFowstTransactionNumber = CInt2(pDrDeviceFowst("DeviceFowstTransactionNumber")) + 1
                'If pDeviceFowstTransactionNumber > 1 Then
                '    pCreator.DeviceFowstTransactionNumber = pDeviceFowstTransactionNumber
                '    mBusiness.LibraryDeviceFowst.UpdateLastNumber("DeviceFowstTransactionNumber", pLine("FowstID"), pLine("DeviceID"), pDeviceFowstTransactionNumber, Nothing)
                'End If
            End If
            If pLine.ContainsKey("CustomerID") _
                AndAlso IsGuid(pLine("CustomerID")) Then

                pCreator.CustomerID = pLine("CustomerID")
            Else
                pCreator.CustomerID = mBusiness.SystemDefaults.CustomerOrganizationID
            End If

            If pLine.ContainsKey("TotalAmount") Then
                pCreator.TotalAmount = CDec2(pLine("TotalAmount"))
            End If
            pCreator.DeliveryAddressID = pLine("DeliveryAddressID")
            pCreator.BillingAddressID = pLine("BillingAddressID")
            pCreator.LastUpdated = ParseDate(pLine("LastUpdated"))
            If pLine.TryGetValue("PrintCount", pStr) Then
                pCreator.PrintCount = CDec2(pStr)
            End If
            If pLine.TryGetValue("IsPrinted", pStr) Then
                pCreator.IsPrinted = CBool2(pStr)
            End If

            If ReadyGuid(OrgID) <> ReadyGuid(modClient.CincoOrgID) _
                AndAlso ReadyGuid(OrgID) <> ReadyGuid(modClient.PotatoCornerFranchiseOrgID) _
                AndAlso ReadyGuid(OrgID) <> ReadyGuid(modClient.PotatoCornerFranchise2OrgID) Then

                pMaxSubNumber = mBusiness.TransInvoice.GetMaxSubNumber(
                vInvoiceNumber:=pLine("InvoiceNumber"),
                vDeviceID:=pLine("DeviceID"),
                vFowstID:=pLine("FowstID"))
                If pMaxSubNumber IsNot Nothing Then
                    pCreator.SubNumber = pMaxSubNumber + 1
                End If
            End If

            pLine.TryGetValue("RefNum", pCreator.RefNum)
            pLine.TryGetValue("ReasonForReconstructingData", pCreator.ReasonForReconstructingData)
            pLine.TryGetValue("InvoiceTypeCode", pCreator.InvoiceTypeCode)

            pCreator.UserID = pLine("UserID")
            If pLine.ContainsKey("Remarks") Then
                pCreator.Remarks = Truncate(pLine("Remarks"), 999)
            End If
            If pLine.ContainsKey("SalesOrderNo") AndAlso CStr2(pLine("SalesOrderNo")).Trim <> "" Then
                pCreator.SalesOrderNo = pLine("SalesOrderNo")
            End If
            If pLine.ContainsKey("SalesOrderDate") AndAlso CStr2(pLine("SalesOrderDate")).Trim <> "" Then
                If ParseDate(pLine("SalesOrderDate")) Is Nothing Then
                    Dim transformDate As String = DateTime.ParseExact(pLine("SalesOrderDate"),
                                        "d/M/yyyy h:mm:ss tt",
                                         System.Globalization.CultureInfo.InvariantCulture).ToString("M/d/yyyy h:mm:ss tt")
                    pCreator.SalesOrderDate = ParseDate(transformDate)
                Else
                    pCreator.SalesOrderDate = ParseDate(pLine("SalesOrderDate"))
                End If
            End If
            If pLine.ContainsKey("SalesOrderUserID") AndAlso CStr2(pLine("SalesOrderUserID")).Trim <> "" Then
                pCreator.SalesOrderUserID = pLine("SalesOrderUserID")
            End If
            If pLine.ContainsKey("CustomerTableNo") AndAlso CStr2(pLine("CustomerTableNo")).Trim <> "" Then
                pCreator.CustomerTableNo = pLine("CustomerTableNo")
            End If
            ' additional by janine june 2
            If pLine.ContainsKey("IsForGovReport") AndAlso CStr2(pLine("IsForGovReport")).Trim <> "" Then
                pCreator.IsForGovReport = pLine("IsForGovReport")
            End If
            If pLine.ContainsKey("IsTestOnly") AndAlso CStr2(pLine("IsTestOnly")).Trim <> "" Then
                pCreator.IsTestOnly = pLine("IsTestOnly")
            End If
            'additional aug 6
            If pLine.ContainsKey("PurchaseOrderNo") AndAlso CStr2(pLine("PurchaseOrderNo")).Trim <> "" Then
                pCreator.PurchaseOrderNo = pLine("PurchaseOrderNo")
            End If
            If pLine.ContainsKey("PurchaseOrderDate") AndAlso CStr2(pLine("PurchaseOrderDate")).Trim <> "" Then
                pCreator.PurchaseOrderDate = pLine("PurchaseOrderDate")
            End If
            If pLine.ContainsKey("DeliveryDate") AndAlso CStr2(pLine("DeliveryDate")).Trim <> "" Then
                If ParseDate(pLine("DeliveryDate")) Is Nothing Then
                    Dim transformDate As String = DateTime.ParseExact(pLine("DeliveryDate"),
                                        "d/M/yyyy h:mm:ss tt",
                                         System.Globalization.CultureInfo.InvariantCulture).ToString("M/d/yyyy h:mm:ss tt")
                    pCreator.SODeliveryDate = ParseDate(transformDate)
                Else
                    pCreator.SODeliveryDate = ParseDate(pLine("DeliveryDate"))
                End If
            End If
            If pLine.ContainsKey("PurchaserName") AndAlso CStr2(pLine("PurchaserName")).Trim <> "" Then
                pCreator.PurchaserName = pLine("PurchaserName")
            End If
            If pLine.ContainsKey("SalesOrderSignatory") AndAlso CStr2(pLine("SalesOrderSignatory")).Trim <> "" Then
                pCreator.SalesOrderSignatory = pLine("SalesOrderSignatory")
            End If
            'oct 24
            If pLine.ContainsKey("DeliveryMethod") AndAlso CStr2(pLine("DeliveryMethod")).Trim <> "" Then
                pCreator.DeliveryMethod = pLine("DeliveryMethod")
            End If
            If pLine.ContainsKey("OrderMethod") AndAlso CStr2(pLine("OrderMethod")).Trim <> "" Then
                pCreator.OrderMethod = pLine("OrderMethod")
            End If
            If pLine.ContainsKey("CustomerDemographicGender") AndAlso CStr2(pLine("CustomerDemographicGender")).Trim <> "" Then
                pCreator.CustomerDemographicGender = pLine("CustomerDemographicGender")
            End If
            If pLine.ContainsKey("CustomerDemographicAge") AndAlso CStr2(pLine("CustomerDemographicAge")).Trim <> "" Then
                pCreator.CustomerDemographicAge = pLine("CustomerDemographicAge")
            End If
            If pLine.ContainsKey("TotalNumberOfCustomer") AndAlso CStr2(pLine("TotalNumberOfCustomer")).Trim <> "" Then
                pCreator.TotalNumberOfCustomer = CDec2(pLine("TotalNumberOfCustomer"))
            End If
            If pLine.ContainsKey("SupplierFowstID") AndAlso CStr2(pLine("SupplierFowstID")).Trim <> "" Then
                pCreator.SupplierFowstID = pLine("SupplierFowstID")
                If IsGuid(pCreator.SupplierFowstID) = False Then
                    AppendToLog("Invoice import data from Org, " & OrgID & ", has an invalid SupplierFowstID, " & pCreator.SupplierFowstID & "." & vbCrLf &
                        "Invoice data:" & vbCrLf &
                        JsonConvert.SerializeObject(pLine, Formatting.Indented), ,
                        "ImportData invalid SupplierFowstID vbVXZNNBDQRJ!")
                    Dim pDtFowst As DataTable
                    pDtFowst = mBusiness.LibraryOrganizationFOWST.GetByOrgID(OrgID)
                    If pDtFowst.Rows.Count > 0 Then
                        pCreator.SupplierFowstID = CStr2(pDtFowst.Rows(0)("ID"))
                    End If
                End If
            End If

            If CInt2(pLine("InvoiceNumber")) > 0 Then
                pCreator.FranchiseeOrgID = IIf(GetFranchiseeOrgFOWSTID(pLine("FowstID")), pLine("FowstID"), Nothing)
            End If
            If pLine.ContainsKey("SupplierID") Then
                pCreator.SupplierID = pLine("SupplierID")
            End If

            If pLine.ContainsKey("ReceivingID") Then
                pCreator.ReceivingID = pLine("ReceivingID")
            End If
            If pLine.ContainsKey("SupplierBinID") Then
                pCreator.SupplierBinID = pLine("SupplierBinID")
            End If
            If pLine.ContainsKey("CustomerFowstID") Then
                pCreator.CustomerFowstID = pLine("CustomerFowstID")
            End If
            If pLine.ContainsKey("CustomerBinID") Then
                pCreator.CustomerBinID = pLine("CustomerBinID")
            End If

            If pLine.ContainsKey("VAT") Then
                pCreator.VAT = pLine("VAT")
            End If

            If pLine.ContainsKey("Vatable") Then
                pCreator.Vatable = pLine("Vatable")
            End If
            If pLine.ContainsKey("VATExempt") Then
                pCreator.VATExempt = pLine("VATExempt")
            End If
            If pLine.ContainsKey("ForegoneVAT") Then
                pCreator.ForegoneVAT = pLine("ForegoneVAT")
            End If
            If pLine.ContainsKey("GrossSalesToRegDiner") Then
                pCreator.GrossSalesToRegDiner = pLine("GrossSalesToRegDiner")
            End If
            If pLine.ContainsKey("GrossSalesToSC") Then
                pCreator.GrossSalesToSC = pLine("GrossSalesToSC")
            End If
            If pLine.ContainsKey("WeightedSCDiscountRate") Then
                pCreator.WeightedSCDiscountRate = pLine("WeightedSCDiscountRate")
            End If
            If pLine.ContainsKey("PhNatTeamDiscount") Then
                pCreator.PhNatTeamDiscount = pLine("PhNatTeamDiscount")
            End If
            If pLine.ContainsKey("SoloParentDiscount") Then
                pCreator.SoloParentDiscount = pLine("SoloParentDiscount")
            End If
            If pLine.ContainsKey("SCDiscount") Then
                pCreator.SCDiscount = pLine("SCDiscount")
            End If
            If pLine.ContainsKey("SCDiscountExempt") Then
                pCreator.SCDiscountExempt = pLine("SCDiscountExempt")
            End If
            If pLine.ContainsKey("TotalNonSCDiscount") Then
                pCreator.TotalNonSCDiscount = pLine("TotalNonSCDiscount")
            End If
            If pLine.ContainsKey("AmountDueBeforeCharges") Then
                pCreator.AmountDueBeforeCharges = pLine("AmountDueBeforeCharges")
            End If
            If pLine.ContainsKey("DepositAmount") Then
                pCreator.DepositAmount = pLine("DepositAmount")
            End If
            If pLine.ContainsKey("AmountDueNetofCharges") Then
                pCreator.AmountDueNetofCharges = pLine("AmountDueNetofCharges")
            End If
            If pLine.ContainsKey("IsNoneVATSale") Then
                pCreator.IsNoneVATSale = CBool2(pLine("IsNoneVATSale"))
            End If
            If pLine.ContainsKey("TransType") Then
                pCreator.TransType = CStr2(pLine("TransType"))
            End If
            If pLine.ContainsKey("IsEmailReceipt") Then
                IsEmailReceipt = CBool2(pLine("IsEmailReceipt"))
                pCreator.IsEmailReceipt = CBool2(pLine("IsEmailReceipt"))
            End If
            If pLine.ContainsKey("ErrorCorrectAmount") Then
                pCreator.ErrorCorrectAmount = CDec2(pLine("ErrorCorrectAmount"))
            End If
            If pLine.ContainsKey("ErrorCorrectCount") Then
                pCreator.ErrorCorrectCount = CDec2(pLine("ErrorCorrectCount"))
            End If
            'end of additional

            'pCreator.ProductID = pLine("ProductID")
            'pCreator.SalesRepList = Common.WebRequest.AllData("SalesRep")
            'pCreator.ProductSerialList = Common.WebRequest.AllData("ProductSerial")

            pIU = pBinder.CreateInsertUpdate(oError) ' not acctually used. just used for checking (see code below)
            If pIU Is Nothing Then
                Continue Do
            End If

            Dim pDeviceFowstTransactionNumber As Integer
            HandleDeviceFowstTransactionNumber(rIU:=pIU, vLine:=pLine, oDeviceFowstTransactionNumber:=pDeviceFowstTransactionNumber)
            pCreator.DeviceFowstTransactionNumber = pDeviceFowstTransactionNumber

            'dtAdjustmentItems = InventoryAdjustment(pLine("FowstID"))

            ''Check for adjustment
            'If (DateDiff(DateInterval.Second, CDate(pLine("InvoiceDate")), dtAdjustmentItems) > 0) Then
            '    'IsHasPossibleReconstruction = True
            'End If

            Try
                mBusiness.ProData.BeginTransaction()
                If mBusiness.ProData.TransDepth <= 0 Then
                    Throw New Exception("You must be in a transaction. vb2X7W86WQBX")
                End If

                If AllowReimportNonZeroInvoice Then
                    AppendToLog("zeroing invoice# " & pLine("InvoiceNumber"), , "Reimport")
                    ' set InvoiceNumber to zero before deleting items
                    pSql = "UPDATE TransInvoice" & vbCrLf &
                        " SET InvoiceNumber = 0" &
                        " WHERE ID = " & ReadyString(pLine("ID"))
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                pSql = "DELETE FROM TransInvoiceItemSalesReps" & vbCrLf &
                    " WHERE InvoiceID = " & ReadyString(pLine("ID"))
                mBusiness.ProData.ExecuteSQL(pSql)

                ' trigger (TransInvoiceItems_trd) will cause error for deleting items from non-zero invoices 
                pSql = "DELETE FROM TransInvoiceItems" & vbCrLf &
                    " WHERE InvoiceID = " & ReadyString(pLine("ID"))
                mBusiness.ProData.ExecuteSQL(pSql)
                If pDtItems Is Nothing Then
                    pCreator.Stocks = Nothing
                Else
                    pCreator.Stocks = New DataView(pDtItems, "InvoiceID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows).ToTable
                    If pDtOutItems Is Nothing Then
                        pDtOutItems = pCreator.Stocks
                    Else
                        pDtOutItems.Merge(pCreator.Stocks)
                    End If
                End If
                If pCreator.Stocks IsNot Nothing Then
                    pIsPo = False
                    pIsPo = pLine.ContainsKey("PurchaseOrderNo") AndAlso CDec2(pLine("PurchaseOrderNo")) <> 0
                    If pIsPo Then
                        For Each pDrItem As DataRow In pCreator.Stocks.Rows
                            If CDec2(pDrItem("Quantity")) = 0 Then
                                pDrItem("Quantity") = pDrItem("OrderedQuantity")
                            End If
                        Next pDrItem
                        mBusiness.TransInvoice.UpdatePoCounter(pLine("DeviceID"), pLine("FowstID"), pLine("PurchaseOrderNo"))
                    End If
                End If

                If AllowReimportNonZeroInvoice Then
                    If pCreator.Stocks Is Nothing Then
                        AppendToLog("invoice# " & pLine("InvoiceNumber") & " has no items", , "Reimport")
                    Else
                        AppendToLog("invoice# " & pLine("InvoiceNumber") & " has " & pCreator.Stocks.Rows.Count & " items", , "Reimport")
                    End If
                End If
                pSW2.Stop()

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vb8SPJDRP82C! before save " & pLine("ID"), , "ImportDataClass2")
                End If
                If pCreator.Save(False, oError) = False Then
                    If DeviceID = modClient.SportsTownOrgID Then
                        AppendToLog("vbS5MFBTFJEX! error save " & pLine("ID"), , "ImportDataClass2")
                    End If
                    AppendToDebug("vbM823DUDP23! OrgID: " & OrgID & vbCrLf & oError)
                    'mBusiness.ProData.RollbackTransaction(True, True)
                    mBusiness.ProData.RollbackTransaction()
                    If CStr2(oError).Contains("vbCEGHYZTB24") Then
                        pDrOut = pDtOut.NewRow
                        pDrOut("ID") = pLine("ID")
                        pDrOut("InvoiceNumber") = pLine("InvoiceNumber")
                        pDrOut("LastUpdated") = pLine("LastUpdated")
                        pDtOut.Rows.Add(pDrOut)
                        Continue Do
                    Else
                        Exit Do
                    End If
                End If
                pSavedInvoiceIDs.Add(ReadyGuid(pCreator.ID))
                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbXHKG2WKUQE! success save " & pLine("ID"), , "ImportDataClass2")
                End If
                pHeaderDuration += pCreator.HeaderStopWatch.ElapsedMilliseconds
                pInvoiceCount += 1
                pItemsDuration += pCreator.ItemsStopWatch.ElapsedMilliseconds
                pItemCount += pCreator.Stocks.Rows.Count

                If ReadyGuid(OrgID) = ReadyGuid(ErpToMobizExporter.OrgID) Then
                    If mBusiness.Utilities.HasWholesaleDatabase Then
                        DataImportLog.AddLog("HasWholesaleDatabase = True")
                        Dim pInvoiceToOrder As InvoiceToOrder
                        pInvoiceToOrder = New InvoiceToOrder(mBusiness.ProData, CreateWholesaleProData())
                        pInvoiceToOrder.Execute(pCreator.ID)
                    Else
                        DataImportLog.AddLog("HasWholesaleDatabase = False")
                    End If
                End If

                mBusiness.TransInvoice.UpdateCounter(pLine("DeviceID"), pLine("FowstID"), pLine("InvoiceNumber"))
                If pLine.ContainsKey("SalesOrderNo") _
                    AndAlso IsNumeric(pLine("SalesOrderNo")) Then

                    mBusiness.TransInvoice.UpdateSoCounter(pLine("DeviceID"), pLine("FowstID"), pLine("SalesOrderNo"))
                End If

                pItemModifiersStopWatch.Start()
                If ImportInvoiceItemsModifier(vInputZipFilename, pLine("ID"), oError) = False Then ' 2017_04_17__13_33_26_1699
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pItemModifiersStopWatch.Stop()
                If CDec2(pLine("InvoiceNumber")) > 0 Then
                    pInventoryStopWatch.Start()
                    mBusiness.TransInvoice.AddToInventoryLedger(pLine("ID"), vOrgID, IsHasPossibleReconstruction)
                    pInventoryStopWatch.Stop()
                End If

                pSalesRepStopWatch.Start()
                If ImportInvoiceItemSalesReps(vInputZipFilename, pLine("ID"), oError, pCreator.Stocks) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pSalesRepStopWatch.Stop()

                pItemRebatesStopWatch.Start()
                If ImportInvoiceItemRebates(vInputZipFilename, pLine("ID"), pLine("FowstID"), pLine("UserID"), oError) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pItemRebatesStopWatch.Stop()

                pItemSerialsStopWatch.Start()
                If ImportInvoiceItemSerials(vInputZipFilename, pLine("ID"), oError) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pItemSerialsStopWatch.Stop()

                pItemDiscountStopWatch.Start()
                If ImportInvoiceDiscount(vInputZipFilename, pLine("ID"), oError) = False Then
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pItemDiscountStopWatch.Stop()

                pSeniorPwdStopWatch.Start()
                If ImportInvoiceSeniorCitizenPWDCustomer(vInputZipFilename, pLine("ID"), oError) = False Then 'additional by janine july 26
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pSeniorPwdStopWatch.Stop()

                pInvoiceChargesStopWatch.Start()
                If ImportInvoiceCharges(vInputZipFilename, pLine("ID"), oError) = False Then 'additional by janine jan 19 2015
                    DataImportLog.AddLog(oError)
                    DataImportLog.Save(DataImportLog.eResultType.Fail)
                    mBusiness.ProData.RollbackTransaction(True, True)
                    Exit Do
                End If
                pInvoiceChargesStopWatch.Stop()

                mBusiness.ProData.CommitTransaction()
                AppendToImportedInvoicesFile(pLine("ID"))
                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbBNRAB796Z6! success in payment save " & pLine("ID"), , "ImportDataClass2")
                End If
                mPaymentStopWatch.Stop()

                If IsInsertToPaymentsTable AndAlso IsEmailReceipt Then
                    pEmailStopWatch.Start()
                    SendEmailReceipt2(pLine("ID"), pDtCustomerPayments, pDtCustomerPaymentInvoices, pLine("DeviceID"))
                    pEmailStopWatch.Stop()
                    pEmailCount += 1
                    'email receipt to customer
                End If
                'Dim DashboardsAndReports = New DashBoardsAndReports

                'DashboardsAndReports.CacheCustomerTransactionSummary(pLine("FowstID"), pLine("CustomerID"), pLine("InvoiceDate"), pLine("InvoiceDate"), mBusiness.ProData)

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")

                If ParseDate(pLine("LastUpdated")) Is Nothing Then
                    Dim transformDate As String = DateTime.ParseExact(pLine("LastUpdated"),
                                        "d/M/yyyy h:mm:ss tt",
                                         System.Globalization.CultureInfo.InvariantCulture).ToString("M/d/yyyy h:mm:ss tt")
                    pDrOut("LastUpdated") = ParseDate(transformDate)
                Else
                    pDrOut("LastUpdated") = ParseDate(pLine("LastUpdated"))
                End If

                pDtOut.Rows.Add(pDrOut)
                If mConsolidationMode Then
                    Dim pIuConsolidate = New InsertUpdate("TransInvoice")
                    pIuConsolidate("ConsolidationDate") = "GETDATE()"
                    pSql = pIuConsolidate.GetUpdate("ID = " & ReadyString(pLine("ID")))
                    pSql = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
                End If
                pMustRecacheInvoice = True

                lInvoiceDate = pLine("InvoiceDate")
                lDeviceID = pLine("DeviceID")
                lFOWSTID = pLine("FowstID")

#If DEBUG Then
                If Now = #1/1/2000# Then
                    Throw New ApplicationException("was deadlocked on lock resources with another process and has been chosen as the deadlock victim")
                End If
#End If
                'InsertImportBenchmark(pSW, pSwBatch, pDtOut.Rows.Count)
                pSW2.Stop()
            Catch ex As Exception
                oError = ex.Message
                pStr = "vb28KBENZMCW! " & GetStackTrace(, ex)
                AppendToLog(pStr, , "ImportInvoice vb28KBENZMCW!")
                mBusiness.ProData.RollbackTransaction(True, True)
                If pDtOut.Rows.Count > 0 Then
                    Exit Do
                Else
                    AppendToDebug("vbLWDB6NFYCA", , ex, True)
                    Throw
                End If
            End Try
        Loop

        If pDtOut.Rows.Count = 0 Then
            If CStr2(oError).Trim <> "" Then
                Return False
            End If
        Else
            '            InsertImportBenchmark(pSW, pSwBatch, pDtOut.Rows.Count)
        End If

        Dim pProcessedPaymentInvoices As New List(Of String)
        For Each pDrCustomerPaymentInvoice As DataRow In pDtCustomerPaymentInvoices.Rows
            If pProcessedPaymentInvoices.Contains(ReadyGuid(pDrCustomerPaymentInvoice("InvoiceID"))) Then
                Continue For
            End If
            If pImportedPaymentInvoices.Contains(ReadyGuid(pDrCustomerPaymentInvoice("InvoiceID"))) Then
                Continue For
            End If
            pProcessedPaymentInvoices.Add(ReadyGuid(pDrCustomerPaymentInvoice("InvoiceID")))
        Next

        For Each pProcessedPaymentInvoice In pProcessedPaymentInvoices
            mBusiness.ProData.BeginTransaction()
            If ImportCustomerPayments(vDT:=pDtCustomerPayments,
                vBinder:=pBinderCustomerPayments,
                rDtOut:=pDtOutCustomerPayments,
                rDtOutInvoices:=pDtCustomerPaymentInvoices,
                oError:=oError,
                vInvoiceID:=pProcessedPaymentInvoice,
                vDtCustomerPaymentInvoices:=pDtCustomerPaymentInvoices,
                vBinderCustomerPaymentInvoices:=pBinderCustomerPaymentInvoices,
                vDtCustomerPaymentDebitCards:=pDtCustomerPaymentDebitCards,
                vBinderCustomerPaymentDebitCards:=pBinderCustomerPaymentDebitCards,
                vDtCustomerPaymentCreditCards:=pDtCustomerPaymentCreditCards,
                vBinderCustomerPaymentCreditCards:=pBinderCustomerPaymentCreditCards,
                vDtCustomerPaymentChecks:=pDtCustomerPaymentChecks,
                vBinderCustomerPaymentChecks:=pBinderCustomerPaymentChecks,
                vDtCustomerPaymentOther:=pDtCustomerPaymentOther,
                vBinderCustomerPaymentOther:=pBinderCustomerPaymentOther) = False Then

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbDJFFWV3BN3! error in payment save " & pProcessedPaymentInvoice & ": " & oError, , "ImportDataClass2")
                End If
                DataImportLog.AddLog(oError & ". Zip=" & vInputZipFilename)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                mBusiness.ProData.RollbackTransaction(True, True)
                Exit For
            End If
            AppendToImportedPaymentInvoicesFile(pProcessedPaymentInvoice)
            mBusiness.ProData.CommitTransaction()
        Next

        For Each pSavedInvoiceID In pSavedInvoiceIDs
            If mCachedInvoices.Contains(pSavedInvoiceID) = False Then
                mBusiness.TransInvoice.CacheInvoicesWithCredit(pSavedInvoiceID, v1stInstance:=If(mBusiness1stInstance IsNot Nothing, mBusiness1stInstance.ProData, Nothing))
            End If
        Next

        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Intro", vTotalDuration:=pSW2.ElapsedMilliseconds, vRecordCount:=0)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Header", vTotalDuration:=pHeaderDuration, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Items", vTotalDuration:=pItemsDuration, vRecordCount:=pItemCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Item Modifiers", vTotalDuration:=pItemModifiersStopWatch.ElapsedMilliseconds, vRecordCount:=mItemModifiersCount)
        'InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Inventory", vTotalDuration:=pInventoryStopWatch.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice InventoryGetCart", vTotalDuration:=mBusiness.TransInvoice.SwInventoryGetCart.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice InventoryMisc", vTotalDuration:=mBusiness.TransInvoice.SwInventoryMisc.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice InventoryModifier", vTotalDuration:=mBusiness.TransInvoice.SwInventoryModifier.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice InventoryIsHasPossibleReconstruction", vTotalDuration:=mBusiness.TransInvoice.SwInventoryIsHasPossibleReconstruction.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice InventoryDataNeedsReconstruction", vTotalDuration:=mBusiness.TransInvoice.SwInventoryDataNeedsReconstruction.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice InventoryAddItemToInventoryLedger", vTotalDuration:=mBusiness.TransInvoice.SwInventoryAddItemToInventoryLedger.ElapsedMilliseconds, vRecordCount:=pInvoiceCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Sales Rep", vTotalDuration:=pSalesRepStopWatch.ElapsedMilliseconds, vRecordCount:=mSalesRepCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Item Rebates", vTotalDuration:=pItemRebatesStopWatch.ElapsedMilliseconds, vRecordCount:=mItemRebatesCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Item Serials", vTotalDuration:=pItemSerialsStopWatch.ElapsedMilliseconds, vRecordCount:=mItemSerialsCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Discount", vTotalDuration:=pItemDiscountStopWatch.ElapsedMilliseconds, vRecordCount:=mInvoiceDiscountCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Senior Citizen / PWD", vTotalDuration:=pSeniorPwdStopWatch.ElapsedMilliseconds, vRecordCount:=mSeniorPwdCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Charges", vTotalDuration:=pInvoiceChargesStopWatch.ElapsedMilliseconds, vRecordCount:=mInvoiceChargesCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Items Voided", vTotalDuration:=pItemsVoidedStopWatch.ElapsedMilliseconds, vRecordCount:=mItemsVoidedCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Payment", vTotalDuration:=mPaymentStopWatch.ElapsedMilliseconds, vRecordCount:=mPaymentCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Payment UpdateZReadCache", vTotalDuration:=mSwUpdateZReadCache.ElapsedMilliseconds, vRecordCount:=mPaymentCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Payment UpdateEJournalCache", vTotalDuration:=mSwUpdateEJournalCache.ElapsedMilliseconds, vRecordCount:=mPaymentCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Payment CacheInvoicesWithCredit", vTotalDuration:=mSwCacheInvoicesWithCredit.ElapsedMilliseconds, vRecordCount:=mPaymentCount)
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice Email", vTotalDuration:=pEmailStopWatch.ElapsedMilliseconds, vRecordCount:=pEmailCount)

        Dim pDtVerified As DataTable

        pSW.Restart()
        If Now <= New Date(2020, 2, 8) Then
            pDtVerified = pDtOut
        Else
            pDtVerified = VerifiedIDs(pDtOut, "TransInvoice")
            If mFromFtp = False Then
                For Each pDrVerified As DataRow In pDtVerified.Rows
                    If CInt2(pDrVerified("InvoiceNumber")) <> 0 Then
                        pDrVerified("LastUpdated") = DBNull.Value
                    End If
                Next
                If pDtVerified.Columns.Contains("InvoiceNumber") Then
                    pDtVerified.Columns.Remove("InvoiceNumber")
                End If
            End If
        End If
        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice after VerifiedIDs", vTotalDuration:=pSW.ElapsedMilliseconds, vRecordCount:=0)
        pSW.Restart()

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtVerified, "TransInvoice")
            If pDtOutCustomerPayments IsNot Nothing Then
                rZipOut.AddTableCsv(pDtOutCustomerPayments, "TransCustomerPayment")
            End If
        End If

        'CheckSendingOfData() wala dapat ni diri, since sa ZREad ang trigger sa pag send og files sa mega world

        InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice after AddTableCsv", vTotalDuration:=pSW.ElapsedMilliseconds, vRecordCount:=0)
        pSW.Restart()

        ' TransInvoiceItems is not tagged anymore. June 12, 2014
        'If pDtOutItems IsNot Nothing _
        '    AndAlso pDtOutItems.Rows.Count > 0 Then

        '    pDtOutItems = New DataView(pDtOutItems, "", "", DataViewRowState.CurrentRows).ToTable(True, "ID")
        '    rZipOut.AddTableCsv(pDtOutItems, "TransInvoiceItems")
        'End If

        If Not lDeviceID Is Nothing Then
            UpdateMonitoringTable(lDeviceID, lFOWSTID, lInvoiceDate, vInputZipFilename)
            InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice after UpdateMonitoringTable", vTotalDuration:=pSW.ElapsedMilliseconds, vRecordCount:=0)
            pSW.Restart()
        End If

        If pDataImportLogAlreadyImported IsNot Nothing Then
            pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
            InsertBenchmark2(vZipFilename:=InputFilename, vProcessName:="Invoice after pDataImportLogAlreadyImported.Save", vTotalDuration:=pSW.ElapsedMilliseconds, vRecordCount:=0)
            pSW.Restart()
        End If

        Return True
    End Function

    Private Sub InsertBenchmark2(vZipFilename As String, vProcessName As String, vTotalDuration As Integer, vRecordCount As Integer)

#If DEBUG Then
        Thread.CurrentThread.Sleep(1)
#End If
        Return ' disabled on 2022_09_22__11_57_36_8901

        Dim pIU = New InsertUpdate("TransImportBenchmark2")
        pIU("ID") = "NEWID()"
        pIU("LastUpdated") = ReadyString(Format(Now, cGlobal.FormatDateTimeSql2)) ' dont use "GETDATE()" here
        pIU("ZipFilename") = ReadyString(vZipFilename)
        pIU("ProcessName") = ReadyString(vProcessName)
        pIU("RecordCount") = vRecordCount ' allow zero
        If vRecordCount <= 0 Then
            vRecordCount = 1 ' dont allow zero or negative
        End If
        pIU("AverageMilliseconds") = CInt2(vTotalDuration / vRecordCount)
        Dim pSql = pIU.GetInsert()
        mBusiness.ProData.ExecuteSQL(pSql)

    End Sub

    Private Sub InsertImportBenchmark(vSw As Stopwatch, vSwBatch As Stopwatch, vRecordCount As Integer)

#If DEBUG Then
        If vSwBatch.ElapsedMilliseconds < 60 * 1000 Then
            Return
        End If
        vSw.Stop()
        Dim pIuImportBenchmark = New InsertUpdate("TransImportBenchmark")
        pIuImportBenchmark("ID") = "NEWID()"
        pIuImportBenchmark("LastUpdated") = "GETDATE()"
        pIuImportBenchmark("MillisecondsPerTrans") = vSw.ElapsedMilliseconds / vRecordCount

        Dim pSql = "SELECT COUNT(*) FROM TransInvoice"
        pIuImportBenchmark("TransInvoiceCount") = CInt2(mBusiness.ProData.ExecuteScalar(pSql))

        pSql = "SELECT COUNT(*) FROM TransInvoiceItems"
        pIuImportBenchmark("TransInvoiceItemsCount") = CInt2(mBusiness.ProData.ExecuteScalar(pSql))

        pSql = "SELECT COUNT(*) FROM TransCustomerPayment"
        pIuImportBenchmark("TransCustomerPaymentCount") = CInt2(mBusiness.ProData.ExecuteScalar(pSql))

        pSql = "SELECT COUNT(*) FROM TransCustomerPaymentInvoices"
        pIuImportBenchmark("TransCustomerPaymentInvoicesCount") = CInt2(mBusiness.ProData.ExecuteScalar(pSql))

        pSql = pIuImportBenchmark.GetInsert()
        mBusiness.ProData.ExecuteSQL(pSql)
        vSw.Start()
        vSwBatch.Restart()
#End If

    End Sub

    Private Function ImportInvoiceItemsVoided(ByVal vInputZipFilename As String, ByRef rZipOut As ProZip, ByRef oError As String) As Boolean
        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pDvProducts As DataView
        oError = ""
        DataImportLog.AddLog("Import from TransInvoiceItemsVoided")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemsVoided")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("ActualQuantity", True, SqlDbType.Decimal)
            .Add("UnitMeasure", True, SqlDbType.VarChar)
            .Add("DateTimeRemoved", True, SqlDbType.DateTime)
            .Add("RemovedByPeopleID", True, SqlDbType.UniqueIdentifier)
            .Add("VoidNumber", False, SqlDbType.Int)
            .Add("DeviceID", False, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.UniqueIdentifier)
            .Add("CashierID", False, SqlDbType.UniqueIdentifier)
            .Add("UnitPrice", False, SqlDbType.Decimal)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            If pIU.Items.ContainsKey("InvoiceID") = False Then
                pIU("InvoiceID") = ReadyString(Guid.NewGuid())
            End If
            pIU.TableName = "TransInvoiceItemsVoided"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            mItemsVoidedCount += 1
        Loop

        rZipOut.AddTableCsv(pDtOut, "TransInvoiceItemsVoided")

        Return True

    End Function

    Private Function ImportTransCustomerPaymentSRV(ByVal vInputZipFilename As String, ByRef rZipOut As ProZip, ByRef oError As String) As Boolean
        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        oError = ""
        DataImportLog.AddLog("Import from TransCustomerPaymentSRV")
        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentSRV")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", False, SqlDbType.UniqueIdentifier)
            .Add("MemoID", True, SqlDbType.UniqueIdentifier)
            .Add("ConsumedAmount", True, SqlDbType.Decimal)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransCustomerPaymentSRV"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        rZipOut.AddTableCsv(pDtOut, "TransCustomerPaymentSRV")

        Return True

    End Function

    Private Function ImportTransZReadCache(ByVal vInputZipFilename As String, ByRef rZipOut As ProZip, ByRef oError As String) As Boolean
        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        oError = ""
        DataImportLog.AddLog("Import from TransZReadCache")
        pCsvReader = New CsvReader(vInputZipFilename, "TransZReadCache")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ZReadCount", False, SqlDbType.Decimal)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU.Items.ContainsKey("ZReadCount") = False _
                AndAlso CInt2(pIU.Items("ZReadCount")) = 0 Then
                Continue Do
            End If
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransZReadCache"
            Try
                pSql = pIU.GetUpdate("SalesDate = " & ReadyDateTime(pLine("SalesDate")) & vbCrLf &
                    " AND DeviceID = " & ReadyString(pLine("DeviceID")) & vbCrLf &
                    " AND FOWSTID = " & ReadyString(pLine("FOWSTID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pIU("SalesDate") = ReadyDateTime(pLine("SalesDate"))
                    pIU("DeviceID") = ReadyString(pLine("DeviceID"))
                    pIU("FOWSTID") = ReadyString(pLine("FOWSTID"))
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                'Catch ex As ProSqlException
                '    oError = "An error occured while inserting/updating data: " & ex.Message & ";" & vbCrLf & ex.Sql & vbCrLf & GetStackTrace(, ex)
                '    AppendToLog(oError, , mLogFile)
                '    oError = JsonDictionary.Serialize("Error", oError)
                '    Return False
            Catch ex As Exception
                'oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                'AppendToLog(oError, , mLogFile)
                'oError = JsonDictionary.Serialize("Error", oError)
                'Return False
            Finally
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            End Try
        Loop

        rZipOut.AddTableCsv(pDtOut, "TransZReadCache")

        Return True

    End Function

    Private Function ImportInvoiceItemsModifier(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean
        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pDvProducts As DataView
        Dim pDtInvoiceItems As DataTable

        oError = ""
        DataImportLog.AddLog("Import from TransInvoiceItemsModifier")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemsModifier")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceItemID", True, SqlDbType.UniqueIdentifier)
            .Add("ItemModifierID", False, SqlDbType.UniqueIdentifier)
            .Add("ModifierID", False, SqlDbType.UniqueIdentifier)
            .Add("InputQty", False, SqlDbType.Decimal)
            .Add("OrderedQty", False, SqlDbType.Decimal)
            .Add("ModifierPrice", False, SqlDbType.Decimal)
            .Add("SubTotal", False, SqlDbType.Decimal)
            .Add("OtherDescription", False, SqlDbType.VarChar)
            .Add("PremiumPrice", False, SqlDbType.Decimal)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")
        pSql = " DELETE FROM TransInvoiceItemsModifier" & vbCrLf &
            " WHERE TransInvoiceItemsModifier.InvoiceItemID IN (" & vbCrLf &
            "   SELECT ID" & vbCrLf &
            "   FROM TransInvoiceItems" & vbCrLf &
            "   WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " )  "
        pSql = mBusiness.ProData.ExecuteSQL(pSql) ' result is used for debugging only

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If pDtInvoiceItems Is Nothing Then
                pSql = "SELECT ID" & vbCrLf &
                    " FROM TransInvoiceItems" & vbCrLf &
                    " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf
                pDtInvoiceItems = mBusiness.ProData.Execute(pSql)
            End If
            Dim pDrsInvoiceItems =
                From pDR As DataRow In pDtInvoiceItems.Rows
                Where ReadyGuid(pDR("ID")) = ReadyGuid(pLine("InvoiceItemID"))
                Select pDR
            If pDrsInvoiceItems.Any() = False Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemsModifier"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
                mItemModifiersCount += 1
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function

    Private Function VerifiedIDs(ByVal vDT As DataTable, ByVal vTableName As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String
        Dim pOut As DataTable

        If vDT.Rows.Count = 0 Then
            Return vDT
        End If
        pDT = ExtractFieldValues(vDT, "ID", Integer.MaxValue)
        For Each pDR As DataRow In pDT.Rows
            pSql = "SELECT ID, LastUpdated " & vbCrLf
            If vTableName.ToLower() = "TransInvoice".ToLower() Then
                pSql &= ",InvoiceNumber" & vbCrLf
            End If
            pSql &= " FROM " & EscapeSqlName(vTableName) &
                " WHERE ID IN (" & pDR(0) & ")"
            pOut = mBusiness.ProData.Execute(pSql)
        Next pDR

        Return pOut

    End Function

    Private Function CreateWholesaleProData() As ProData

        Return ProData.CreateFromConnectionInfo(
            vServer:=Common.ConnectionInfo.Server,
            vDatabase:="wholesale",
            vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity,
            vUsername:=Common.ConnectionInfo.Username,
            vPassword:=Common.ConnectionInfo.Password,
            vNoProgressBar:=True)

    End Function

    Private Function CreateInvoiceItemsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pProductID As String

        ' BEGIN get SKU data from database
        'Dim pDtStocks As DataTable
        'Dim pDvStocks As DataView
        'Dim pDrvStock As DataRowView
        ' END get SKU data from database

        DataImportLog.AddLog("Import from TransInvoiceItems")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItems")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            '.Add("PriceLevelID", Not ImportInvoiceOnly, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("TaxSales", False, SqlDbType.Money)
            .Add("SubTotal", True, SqlDbType.Money)
            .Add("LineNumber", False, SqlDbType.Int)
            .Add("OrderedQuantity", False, SqlDbType.Decimal) ' additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", False, SqlDbType.UniqueIdentifier) ' should be true; false to force import
            .Add("ActualQuantity", True, SqlDbType.Int)
            .Add("UnitPrice", True, SqlDbType.Money)
            .Add("OrignalUnitPrice", False, SqlDbType.Money) ' required but be optional for backwards compatibility
            .Add("TaxAmount", False, SqlDbType.Money)
            .Add("TaxPercent", False, SqlDbType.Money)
            '.Add("UnitMeasure", Not ImportInvoiceOnly, SqlDbType.VarChar) ' should be true; false to force import
            .Add("UnitMeasure", False, SqlDbType.VarChar) ' should be true; false to force import

            .Add("DiscountAmount", False, SqlDbType.Money)
            .Add("DiscountSales", False, SqlDbType.Money)
            .Add("RequestedUnitPrice", False, SqlDbType.Money)
            .Add("VATExemptSale", False, SqlDbType.Money)
            .Add("ZeroRatedSale", False, SqlDbType.Money)
            .Add("NonVATSale", False, SqlDbType.Money)
            .Add("VATableSale", False, SqlDbType.Money)
            .Add("ModifierGroupID", False, SqlDbType.UniqueIdentifier)
            .Add("BinID", False, SqlDbType.UniqueIdentifier)
            .Add("ThirdPartyProductNumber", False, SqlDbType.VarChar)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("InvoiceID")
            .Add("ProductID")
            .Add("OrderedQuantity", GetType(Decimal)) 'TODO additional by janine sept 6 2014 for sales order taker
            .Add("Quantity", GetType(Decimal))
            .Add("ActualQuantity", GetType(Integer))
            .Add("UnitMeasure")
            .Add("TermDiscount", GetType(Decimal))
            .Add("DiscountIfPaidWithIn", GetType(Decimal))
            .Add("NetDue")
            .Add("UnitPrice", GetType(Decimal))
            .Add("OriginalUnitPrice", GetType(Decimal))
            .Add("Subtotal", GetType(Decimal))
            .Add("Cost", GetType(Decimal))
            .Add("PriceLevelID")
            .Add("Dimension")
            .Add("IsReplacement", GetType(Boolean))
            .Add("IsHasReplacement", GetType(Boolean))
            .Add("IsHasBackOrder", GetType(Boolean))
            .Add("RefStockID")
            .Add("UnitGP", GetType(Decimal))
            .Add("UnitGPM", GetType(Decimal))
            .Add("LineGP", GetType(Decimal))
            .Add("LineGPM", GetType(Decimal))
            .Add("LineCost", GetType(Decimal))
            .Add("LineNumber", GetType(Integer))
            .Add("TaxPercent", GetType(Decimal))
            .Add("TaxAmount", GetType(Decimal))
            .Add("TaxSales", GetType(Decimal))
            '.Add("SalesRep", GetType(Object))
            .Add("DiscountAmount", GetType(Decimal))
            .Add("DiscountSales", GetType(Decimal))
            .Add("RequestedUnitPrice", GetType(Decimal))
            .Add("VATExemptSale", GetType(Decimal))
            .Add("ZeroRatedSale", GetType(Decimal))
            .Add("NonVATSale", GetType(Decimal))
            .Add("VATableSale", GetType(Decimal))
            .Add("ModifierGroupID")
            .Add("BinID")
            .Add("ThirdPartyProductNumber")
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                AppendToLog("Validate error: " & oError & vbCrLf &
                    JsonConvert.SerializeObject(pLine, Formatting.Indented), ,
                    "CreateInvoiceItemsDataTable vbVC4LZYR7JF!")
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)

            'If ImportInvoiceOnly Then
            If CStr2(pDR("PriceLevelID")).Trim() = "" Then
                AppendToLog("vbTJKFPT8A5M! No PriceLevelID;" & vbCrLf &
                    "ID: " & pLine("ID") & "; " & vbCrLf &
                    "InvoiceID: " & pLine("InvoiceID") & "; " & vbCrLf &
                    "filename: " & InputFilename, , mLogFile)
                pDR("PriceLevelID") = mBusiness.SystemDefaults.GetRetailPriceID()
                'AppendToDebug("vbPN5RXKYB62! No PriceLevelID;" & vbCrLf &
                '    "filename: " & InputFilename)
            End If
            'End If
            If CStr2(pDR("UnitMeasure")).Trim() = "" Then
                AppendToLog("vbLKWXDVTJKA! No UnitMeasure;" & vbCrLf &
                    "ID: " & pLine("ID") & "; " & vbCrLf &
                    "InvoiceID: " & pLine("InvoiceID") & "; " & vbCrLf &
                    "filename: " & InputFilename, , mLogFile)
                pDR("UnitMeasure") = "Piece"
            End If
            If CStr2(pDR("ProductID")).Trim() = "" Then
                AppendToLog("vb6SKYPJA2GF! No ProductID;" & vbCrLf &
                    "ID: " & pLine("ID") & "; " & vbCrLf &
                    "InvoiceID: " & pLine("InvoiceID") & "; " & vbCrLf &
                    "Price: " & pLine("UnitPrice") & "; " & vbCrLf &
                    "ActualQuantity: " & pLine("ActualQuantity") & "; " & vbCrLf &
                    "OrgID: " & OrgID & "; " & vbCrLf &
                    "filename: " & InputFilename, , mLogFile)
                Try
                    pProductID = mBusiness.TransInvoice.GetProductIdByPrice(OrgID, pLine("UnitPrice") / pLine("ActualQuantity"), CStr2(pDR("PriceLevelID")))
                    If String.IsNullOrWhiteSpace(pProductID) Then
                        pProductID = mBusiness.LibraryStock.GetIDByPrice(OrgID, pLine("UnitPrice") / pLine("ActualQuantity"), CStr2(pDR("PriceLevelID")))
                        If String.IsNullOrWhiteSpace(pProductID) Then
                            oError = "vbHR7NFNXFYY! ProductID still not found"
                            AppendToLog(oError, , mLogFile)
                            Return Nothing
                        End If
                    End If
                    If String.IsNullOrWhiteSpace(pProductID) = False Then
                        pProductID = ReadyGuid(pProductID)
                        pDR("ProductID") = pProductID
                    End If
                Catch ex As Exception
                    AppendToLog("vb8RYKT8ZF9X! " & ex.Message, , mLogFile)
                End Try
            End If

            ' BEGIN get SKU data from database
            'pDtStocks = mBusiness.LibraryStock.PriceTable(OrgID, pDR("ProductID"))
            'pDvStocks = New DataView(pDtStocks, "Quantity = " & CDec2(pDR("ActualQuantity")) &
            '    " AND PriceLevelID = " & ReadyString(pDR("PriceLevelID")), "", DataViewRowState.CurrentRows)
            'If pDvStocks.Count > 0 Then
            '    pDrvStock = pDvStocks(0)
            ' END get SKU data from database

            If CDec2(pDR("Quantity")) >= 0 Then
                'pDR("ActualQuantity") = pDrvStock("Quantity")
                'pDR("UnitMeasure") = pDrvStock("UnitMeasure")
                'pDR("UnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
                ' check test
                '    If Math.Abs(pDR("UnitPrice") - pLine("UnitPrice")) > 0.01 Then
                '       Throw New Exception("Conflicting values in unit price vbQV7L2A8VEY")
                '   End If
                'pDR("OriginalUnitPrice") = Math.Round(pDrvStock("DiscountedPrice"), 2)
                pDR("Cost") = mBusiness.LibraryStock.GetLandedCost(pDR("ProductID"), OrgID) * pDR("ActualQuantity")
                'If ReadyGuid(pDrvStock("PriceLevelID")) <> pRetailID Then
                '    'todo: add security
                '    'If WebSession.Instance.UserAccess.PointOfSales_PriceLevelMode.IsEnable _
                '    '    OrElse WebSession.Instance.UserAccess.PointOfSales_UnlimitedUserMode.IsEnable Then

                '    'Else
                '    '    Common.WebRequest.ProData.RollbackTransaction()
                '    '    Return JsonDictionary.Serialize("Error", "You are not authorized to set price level other than retail")
                '    'End If
                'End If
                'pDR("PriceLevelID") = CStr2(pDrvStock("PriceLevelID"))
                pDR("Dimension") = ""
                pDR("RequestedUnitPrice") = pDR("RequestedUnitPrice")
                pDR("IsReplacement") = False
                pDR("IsHasReplacement") = False
                pDR("IsHasBackOrder") = False
                pDR("RefStockID") = ""

                Dim pUnitGp As Decimal
                Dim pUnitGpm As Double
                Dim pLineGp As Decimal
                Dim pLineGpm As Decimal
                Dim pLandedCost As Decimal

                'additional by janine sept 6 
                If (CDec2(pDR("Quantity")) > 0) Then
                    mBusiness.LibraryStock.GetProfitability(
                    vStockID:=CStr2(pDR("ProductID")),
                    vQty:=pDR("Quantity"),
                    vUnitQty:=pDR("ActualQuantity"),
                    vUnitPrice:=pDR("UnitPrice"),
                    oUnitGP:=pUnitGp,
                    oUnitGPM:=pUnitGpm,
                    oLineGP:=pLineGp,
                    oLineGPM:=pLineGpm,
                    oLandedCost:=pLandedCost,
                    vHistoricCost:=0,
                    vOrgID:=OrgID)

                Else
                    mBusiness.LibraryStock.GetProfitability(
                    vStockID:=CStr2(pDR("ProductID")),
                    vQty:=CDec2(pDR("OrderedQuantity")),
                    vUnitQty:=CDec2(pDR("ActualQuantity")),
                    vUnitPrice:=pDR("UnitPrice"),
                    oUnitGP:=pUnitGp,
                    oUnitGPM:=pUnitGpm,
                    oLineGP:=pLineGp,
                    oLineGPM:=pLineGpm,
                    oLandedCost:=pLandedCost,
                    vHistoricCost:=0,
                    vOrgID:=OrgID)
                End If
                '''''''''''''''''''''''''''''''''''

                pDR("UnitGP") = pUnitGp
                pDR("UnitGPM") = pUnitGpm
                pDR("LineGP") = pLineGp
                pDR("LineGPM") = pLineGpm
                pDR("LineCost") = pLandedCost * CDec2(pDR("Quantity")) * CDec2(pDR("ActualQuantity"))

                'additional by janine sept 6 
                If (CDec2(pDR("Quantity")) > 0) Then
                    pDR("Subtotal") = pDR("Quantity") * pDR("UnitPrice")
                Else
                    pDR("Subtotal") = CDec2(pDR("OrderedQuantity")) * CDec2(pDR("UnitPrice"))
                End If
                '''''''''''''''''
                'pDR("Subtotal") = pDR("Quantity") * pDR("UnitPrice") 'commented by janine

                pDR("TaxPercent") = 12 ' todo

                If CDec2(pDR("TaxSales")) = 0 _
                    AndAlso CDec2(pDR("TaxAmount")) = 0 Then

                    'addtional by janine feb 4 2015
                    'if SCD (vat exempt, tax sales and vat amount must be 0)
                    If (CDec2(pDR("VATExemptSale")) = 0 And CDec2(pDR("ZeroRatedSale")) = 0 And CDec2(pDR("NonVATSale")) = 0) Then
                        pDR("TaxSales") = RoundDown(pDR("SubTotal") / (1 + pDR("TaxPercent") / 100), 4)
                        pDR("TaxAmount") = pDR("SubTotal") - pDR("TaxSales")
                    End If
                End If
            Else
                oError = "No matching unit quantity and price level. " & vbCrLf &
                    "Data: " & JsonConvert.SerializeObject(pLine, Formatting.Indented)
                Return Nothing
            End If

            ' BEGIN get SKU data from database
            'End If
            ' END get SKU data from database

            pDtOut.Rows.Add(pDR)
        Loop
        pDtOut.Columns("ProductID").ColumnName = "StockID"

        Return pDtOut

    End Function

    Private Function ImportInvoiceItemSalesReps(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String, vDtProducts As DataTable) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pDvProducts As DataView

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceItemSalesReps")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemSalesReps")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceItemID", False, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceItemSalesReps" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)


        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pDvProducts = New DataView(vDtProducts, "ID = " & ReadyString(pLine("InvoiceItemID")) & " AND InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
            If pDvProducts.Count = 0 Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemSalesReps"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
            mSalesRepCount += 1
        Loop

        Return True

    End Function

    Private Function ImportInvoiceItemRebates(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        vFowstID As String, vUserID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pDtInvoiceItems As DataTable
        Dim pDvModifers As DataView

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceItemsRebates")

        pSql = "DELETE FROM TransInvoiceItemsRebates" & vbCrLf &
            " WHERE TransInvoiceItemsRebates.ModifierGroupID IN" & vbCrLf &
            " (SELECT TransInvoiceItems.ModifierGroupID" & vbCrLf &
            "   FROM TransInvoiceItems" & vbCrLf &
            "   WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            ")"
        pSql = mBusiness.ProData.ExecuteSQL(pSql) ' the output is for debugging only

        pSql = "SELECT ModifierGroupID" & vbCrLf &
            ",SUM(Quantity * ActualQuantity) AS Quantity" & vbCrLf &
            " FROM TransInvoiceItems" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID) & vbCrLf &
            " AND ModifierGroupID IS NOT NULL" & vbCrLf &
            " GROUP BY ModifierGroupID"
        pDtInvoiceItems = mBusiness.ProData.Execute(pSql)
        If pDtInvoiceItems.Rows.Count = 0 Then
            Return True
        End If

        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemsRebates")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("RebateID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Amount", True, SqlDbType.Money)
            .Add("UnitMeasure", True, SqlDbType.VarChar)
            .Add("UnitQuantity", True, SqlDbType.Decimal)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("ModifierGroupID", True, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            pDvModifers = New DataView(pDtInvoiceItems, "ModifierGroupID = " & ReadyString(pLine("ModifierGroupID")), "", DataViewRowState.CurrentRows)
            If pDvModifers.Count = 0 Then
                Continue Do
            End If

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemsRebates"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                mBusiness.TransInvoice.AddItemToInventoryLedger(
                    vProductID:=CStr2(pLine("ProductID")),
                    vOrgID:=OrgID,
                    vFowstID:=vFowstID,
                    vTransactionID:=vInvoiceID,
                    vUserID:=vUserID,
                    vQty:=CDec2(pLine("UnitQuantity")) * CDec2(pLine("Quantity")) * CDec2(pDvModifers(0)("Quantity")),
                    vSubTransactionType:="Return"
                )
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
            mItemRebatesCount += 1
        Loop

        Return True

    End Function

    Private Function ImportInvoiceItemSerials(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceItemSerials")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceItemSerials")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("SerialNumber", True, SqlDbType.VarChar)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceItemSerials" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceItemSerials"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
            mItemSerialsCount += 1
        Loop

        Return True

    End Function

    Private Function ImportInvoiceDiscount(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceDiscount")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceDiscount")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("DiscountPercentage", True, SqlDbType.Float)
            .Add("DiscountTypeID", True, SqlDbType.UniqueIdentifier)
            .Add("DiscountRemarks", False, SqlDbType.VarChar)
            .Add("NoOfPeople", False, SqlDbType.Int) ''additional by janine july 26
            .Add("NoOfSeniorCitizens", False, SqlDbType.Int) ''additional by janine july 26
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceDiscount" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceDiscount"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                Dim pDrAdjustment = mBusiness.TransInvoice.GetAdjustmentBySourceDiscountID(pLine("ID"))
                If pDrAdjustment Is Nothing Then
                    Dim pAdjustmentInput As New TransInvoice.SaveAdjustmentJson
                    pAdjustmentInput.IsNew = True
                    pAdjustmentInput.InvoiceID = vInvoiceID
                    Dim pDrReason = mBusiness.LibraryAccountAdjustments.GetByReason("Sales Discount")
                    If pDrReason Is Nothing Then
                        mBusiness.LibraryAccountAdjustments.AddTypesIfNeeded()
                        pDrReason = mBusiness.LibraryAccountAdjustments.GetByReason("Sales Discount")
                    End If
                    pAdjustmentInput.ReasonID = CStr2(pDrReason("ID"))
                    Dim pDrInvoice = mBusiness.TransInvoice.ById(vInvoiceID)
                    pAdjustmentInput.Amount = CDec2(pDrInvoice("TotalAmount")) * CDec2(pLine("DiscountPercentage")) / 100
                    If String.IsNullOrWhiteSpace(pLine("DiscountRemarks")) = False Then
                        pAdjustmentInput.Remarks = CStr2(pLine("DiscountRemarks")).Trim() & " - "
                    End If

                    pSql = "SELECT DiscountType" & vbCrLf &
                        " FROM LibraryDiscountSettings" & vbCrLf &
                        " WHERE ID = " & ReadyString(pLine("DiscountTypeID"))
                    Dim pDrDiscount = mBusiness.ProData.ExecuteRow(pSql)
                    If pDrDiscount IsNot Nothing Then
                        pAdjustmentInput.Remarks += CStr2(pDrDiscount("DiscountType"))
                    End If
                    pAdjustmentInput.SourceDiscountID = pLine("ID")
                    pAdjustmentInput.UserID = CStr2(pDrInvoice("UserID"))
                    pAdjustmentInput.DontRecache = True
                    mBusiness.TransInvoice.SaveAdjustmentInternal(pAdjustmentInput, oError) ' TransInvoice.SaveAdjustmentInternal calls TransInvoice.CacheInvoicesWithCredit()
                    If String.IsNullOrWhiteSpace(oError) = False Then
                        Return False
                    End If
                End If

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
            mInvoiceDiscountCount += 1
        Loop

        Return True

    End Function

    ''additional by janine july 26
    Private Function ImportInvoiceSeniorCitizenPWDCustomer(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceSeniorCitizenPWDCustomer")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceSeniorCitizenPWDCustomer")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("IDImage", False, SqlDbType.VarChar)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceSeniorCitizenPWDCustomer" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            If pLine.ContainsKey("CustomerID") _
                AndAlso IsGuid(pLine("CustomerID")) Then

                ' just do nothing here
            Else
                pLine("CustomerID") = mBusiness.SystemDefaults.CustomerOrganizationID
            End If
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceSeniorCitizenPWDCustomer"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
            mSeniorPwdCount += 1
        Loop

        Return True

    End Function
    'end of additional

    ''additional by janine jan 19 2015
    Private Function ImportInvoiceCharges(ByVal vInputZipFilename As String, ByVal vInvoiceID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransInvoiceCharges")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInvoiceCharges")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
            .Add("ChargeType", False, SqlDbType.VarChar)
            .Add("ChargeAmount", False, SqlDbType.Money)
            .Add("TipAmount", False, SqlDbType.Money)
            .Add("TaxAmount", False, SqlDbType.Money)
            .Add("Remarks", False, SqlDbType.VarChar)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransInvoiceCharges" & vbCrLf &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("InvoiceID")) <> ReadyGuid(vInvoiceID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransInvoiceCharges"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
            mInvoiceChargesCount += 1
        Loop

        Return True

    End Function
    'end of additional

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vInputZipFilename"></param>
    ''' <param name="oBinder"></param>
    ''' <param name="oError"></param>
    ''' <param name="vDtInvoice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateCustomerPaymentsDataTable(ByVal vInputZipFilename As String,
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String,
        vDtInvoice As DataTable) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        DataImportLog.AddLog("Import from TransCustomerPayment")
        If vDtInvoice Is Nothing Then
            pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPayment")
        Else
        End If
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerID", False, SqlDbType.UniqueIdentifier) ' should be true
            .Add("UserID", True, SqlDbType.UniqueIdentifier)
            .Add("CheckPaymentAmount", True, SqlDbType.Money)
            .Add("AmountChange", True, SqlDbType.Money)
            .Add("CashAmount", True, SqlDbType.Money)
            .Add("SRVAmount", True, SqlDbType.Money)
            .Add("OtherTypeAmount", False, SqlDbType.Money)
            .Add("ChargeAmount", False, SqlDbType.Money) 'new
            .Add("CreditCardAmount", True, SqlDbType.Money)
            .Add("CreditCardVat", False, SqlDbType.Money)
            .Add("DiscountAmount", True, SqlDbType.Money)
            .Add("DebitCardAmount", True, SqlDbType.Money)
            .Add("TotalAmountDue", True, SqlDbType.Money)
            .Add("CustomerChangeCash", True, SqlDbType.Money)
            .Add("ReceiptType", True, SqlDbType.VarChar)
            .Add("ReceiptNumber", True, SqlDbType.VarChar)
            .Add("PaymentDate", True, SqlDbType.DateTime)
            .Add("OnCreditAmount", False, SqlDbType.Money)
            .Add("OnCreditRemarks", False, SqlDbType.VarChar)
            .Add("RewardsCardNum", False, SqlDbType.VarChar)
            .Add("CustomerEmailAddresses", False, SqlDbType.VarChar)
        End With
        If mConsolidationMode Then
            Return Nothing
        End If
        pDtOut = oBinder.CreateDataTable

        If vDtInvoice Is Nothing Then
            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                If pLine.ContainsKey("CustomerID") _
                    AndAlso IsGuid(pLine("CustomerID")) Then

                    ' just do nothing here
                Else
                    pLine("CustomerID") = mBusiness.SystemDefaults.CustomerOrganizationID
                End If
                oBinder.Source = pLine
                pIU = oBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    oError = "TransCustomerPayment: " & oError
                    Return Nothing
                End If

                pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
            Loop
        Else
            Dim pDtInvoiceIDs = ExtractFieldValues(vDtInvoice, "ID", Int32.MaxValue)

        End If


        Return pDtOut

    End Function

    Private Function ImportCustomerPayments(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByRef rDtOut As DataTable,
        ByRef rDtOutInvoices As DataTable,
        ByRef oError As String,
        ByVal vInvoiceID As String,
        ByVal vDtCustomerPaymentInvoices As DataTable,
        ByVal vBinderCustomerPaymentInvoices As DictionaryBinder,
        ByVal vDtCustomerPaymentDebitCards As DataTable,
        ByVal vBinderCustomerPaymentDebitCards As DictionaryBinder,
        ByVal vDtCustomerPaymentCreditCards As DataTable,
        ByVal vBinderCustomerPaymentCreditCards As DictionaryBinder,
        ByVal vDtCustomerPaymentChecks As DataTable,
        ByVal vBinderCustomerPaymentChecks As DictionaryBinder,
        ByVal vDtCustomerPaymentOther As DataTable,
        ByVal vBinderCustomerPaymentOther As DictionaryBinder) As Boolean

        Dim pDvCustomerPayments As DataView
        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCustomerPaymentInvoices As DataView
        Dim pDtCustomerPaymentsUnique As DataTable
        Dim pDtFieldValues As DataTable
        Dim pIds As String

        DataImportLog.AddLog("Import from TransCustomerPayment")
        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If

        pDvCustomerPaymentInvoices = New DataView(vDtCustomerPaymentInvoices, "InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
        If pDvCustomerPaymentInvoices.Count = 0 Then
            ' this invoice has no payment
            ' valid during on order taker
            Return True
        End If
        pDtCustomerPaymentsUnique = pDvCustomerPaymentInvoices.ToTable(True, {"CustomerPaymentID"})
        pDtFieldValues = ExtractFieldValues(pDtCustomerPaymentsUnique, "CustomerPaymentID", Integer.MaxValue)
        If pDtFieldValues.Rows.Count = 0 Then
            ' no matching customer payment ID has been found.
            ' todo: trap this data error
            Stop2()
            Return True
        End If
        pIds = pDtFieldValues.Rows(0)(0)

        pDvCustomerPayments = New DataView(vDT, "CONVERT(ID, 'System.String') IN (" & pIds & ")", "", DataViewRowState.CurrentRows)
        For Each pDrvIn As DataRowView In pDvCustomerPayments
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Continue For
            End If
            pDvCustomerPaymentInvoices = New DataView(vDtCustomerPaymentInvoices, "CustomerPaymentID = " & ReadyString(pDrvIn("ID")) & " AND InvoiceID = " & ReadyString(vInvoiceID), "", DataViewRowState.CurrentRows)
            If pDvCustomerPaymentInvoices.Count = 0 Then
                ' you should panic here
                Stop2()
            End If
            If ReadyGuid(pDrvIn("ID")) = ReadyGuid("0d91d7ef-d040-4ed6-802c-eb0c32a75108") Then
                Continue For
            End If

            pIU.TableName = "TransCustomerPayment"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Dim pIsInsert As Boolean = False
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pIsInsert = True
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                    If CDec2(pDrvIn("OnCreditAmount")) > 0 Then
                        HasOnCreditPayment = True
                    End If
                    IsInsertToPaymentsTable = True
                End If
                pMustRecacheInvoice = True
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & ";" & vbCrLf & pSql & vbCrLf & GetStackTrace(, ex)
                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbS5VBDKDRB3! error in ImportCustomerPayments: " & oError, , "ImportDataClass2")
                End If
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try

            If ImportCustomerPaymentInvoices(
                vDT:=pDvCustomerPaymentInvoices.ToTable(),
                vBinder:=vBinderCustomerPaymentInvoices,
                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
                oError:=oError,
                rDtOut:=rDtOutInvoices,
                vIsInsert:=pIsInsert) = False Then

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbGWL4S4L3Q9! error in ImportCustomerPayments: " & oError, , "ImportDataClass2")
                End If
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If ImportCustomerPaymentDebitCard(
                vDT:=vDtCustomerPaymentDebitCards,
                vBinder:=vBinderCustomerPaymentDebitCards,
                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
                oError:=oError,
                rDtOut:=rDtOutInvoices) = False Then

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbKBEYT99TU8! error in ImportCustomerPayments: " & oError, , "ImportDataClass2")
                End If
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If ImportCustomerPaymentCreditCard(
                vDT:=vDtCustomerPaymentCreditCards,
                vBinder:=vBinderCustomerPaymentCreditCards,
                vCustomerPaymentID:=CStr2(pDrvIn("ID")),
                oError:=oError,
                rDtOut:=rDtOutInvoices) = False Then

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbCGWMHSP56L! error in ImportCustomerPayments: " & oError, , "ImportDataClass2")
                End If
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If ImportCustomerPaymentChecks(
               vDT:=vDtCustomerPaymentChecks,
               vBinder:=vBinderCustomerPaymentChecks,
               vCustomerPaymentID:=CStr2(pDrvIn("ID")),
               oError:=oError,
               rDtOut:=rDtOutInvoices) = False Then

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbJ7AT6PGW8E! error in ImportCustomerPayments: " & oError, , "ImportDataClass2")
                End If
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
            If ImportCustomerPaymentOther(
               vDT:=vDtCustomerPaymentOther,
               vBinder:=vBinderCustomerPaymentOther,
               vCustomerPaymentID:=CStr2(pDrvIn("ID")),
               oError:=oError,
               rDtOut:=rDtOutInvoices) = False Then

                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbTA2WM4LG47! error in ImportCustomerPayments: " & oError, , "ImportDataClass2")
                End If
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If

            If Now.Date = #4/17/2000# Then
                Return False ' drag here to debug
            End If

            If mConsolidationMode Then
                Dim pIuConsolidate = New InsertUpdate("TransCustomerPayment")
                pIuConsolidate("ConsolidationDate") = "GETDATE()"
                pSql = pIuConsolidate.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
                pSql = mBusinessSource.ProData.ExecuteSQL(pSql) ' output is used for debugging only
            End If
            pDrOut = rDtOut.NewRow
            pDrOut("ID") = CStr2(pDrvIn("ID"))
            rDtOut.Rows.Add(pDrOut)
            mPaymentCount += 1
        Next pDrvIn

        'If HasOnCreditPayment Then
        mPaymentStopWatch.Stop()
        mSwCacheInvoicesWithCredit.Start()
        mBusiness.TransInvoice.CacheInvoicesWithCredit(vInvoiceID, v1stInstance:=If(mBusiness1stInstance IsNot Nothing, mBusiness1stInstance.ProData, Nothing))
        mCachedInvoices.Add(ReadyGuid(vInvoiceID))
        mSwCacheInvoicesWithCredit.Stop()
        mPaymentStopWatch.Start()
        'End If
        Return True

    End Function

    Private Function CreateCustomerPaymentInvoicesDataTable(ByVal vInputZipFilename As String,
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentInvoices")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
            .Add("Balance", True, SqlDbType.Money)
            .Add("DiscountAmount", True, SqlDbType.Money)
            .Add("InvoiceAmount", True, SqlDbType.Money)
            .Add("AmountPaid", True, SqlDbType.Money)
        End With
        If mConsolidationMode Then
            Return Nothing
        End If
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentInvoices(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String,
        ByVal vIsInsert As Boolean) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCustomerPaymentInvoices As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If

        pDvCustomerPaymentInvoices = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        ' ToOptimize: delete only when needed
        pSql = "DELETE FROM TransCustomerPaymentInvoices" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvCustomerPaymentInvoices
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vb5UGV8FWBDM! error in ImportCustomerPaymentInvoices: " & oError, , "ImportDataClass2")
                End If
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentInvoices"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)

                    mPaymentStopWatch.Stop()
                    If vIsInsert Then
                        mSwUpdateZReadCache.Start()
                        If UpdateZReadCache(CStr2(pDrvIn("InvoiceID"))) = False Then
                            If DeviceID = modClient.SportsTownOrgID Then
                                AppendToLog("vb6DEVG6JBNM! error in UpdateZReadCache: " & oError, , "ImportDataClass2")
                            End If
                            DataImportLog.AddLog(oError)
                            DataImportLog.Save(DataImportLog.eResultType.Fail)
                            If mIgnoreZReadCacheError = False Then
                                Return False
                            End If
                        End If
                        mSwUpdateZReadCache.Stop()

                        mSwUpdateEJournalCache.Start()
                        If UpdateEJournalCache(CStr2(pDrvIn("InvoiceID"))) = False Then
                            If DeviceID = modClient.SportsTownOrgID Then
                                AppendToLog("vbX4P4DTFQ42! error in UpdateEJournalCache: " & oError, , "ImportDataClass2")
                            End If
                            DataImportLog.AddLog(oError)
                            DataImportLog.Save(DataImportLog.eResultType.Fail)
                            If mIgnoreZReadCacheError = False Then
                                Return False
                            End If
                        End If
                        mSwUpdateEJournalCache.Stop()
                    End If

                    mPaymentStopWatch.Start()
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                If DeviceID = modClient.SportsTownOrgID Then
                    AppendToLog("vbZ8HGA4PM4E! error in catch inside ImportCustomerPaymentInvoices: " & oError, , "ImportDataClass2")
                End If
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    Private Function CreateCustomerPaymentDebitCardDataTable(ByVal vInputZipFilename As String,
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentDebitCard")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("DebitCardType", False, SqlDbType.VarChar)
            .Add("DebitCardBankID", False, SqlDbType.UniqueIdentifier)
            .Add("DebitCardBankName", False, SqlDbType.VarChar)
            .Add("IsBankFromOrg", True, SqlDbType.Bit)
            .Add("BankCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("HolderLastName", False, SqlDbType.VarChar)
            .Add("HolderFirstName", False, SqlDbType.VarChar)
            .Add("HolderMiddleName", False, SqlDbType.VarChar)
            .Add("HolderGenerationalSuffix", False, SqlDbType.VarChar)
            .Add("DebitCardNumber", True, SqlDbType.VarChar)
            .Add("DebitCardIssuanceDate", False, SqlDbType.Date)
            .Add("DebitCardExpirationDate", False, SqlDbType.DateTime)
            .Add("ApprovalNo", False, SqlDbType.VarChar)
            .Add("NoOfInstallmentPayments", False, SqlDbType.Int)
            .Add("DebitCardSurcharge", False, SqlDbType.Float)
            .Add("DebitCardAmount", False, SqlDbType.Money)
        End With
        If mConsolidationMode Then
            Return Nothing
        End If
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentDebitCard(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvDebitCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvDebitCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        pSql = "DELETE FROM TransCustomerPaymentDebitCard" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvDebitCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If CStr2(pDrvIn("HolderLastName")).Length > 50 Then
                pIU("HolderLastName") = ReadyString(CStr2(pDrvIn("HolderLastName")).Substring(0, 50))
            End If
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentDebitCard"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    Private Function CreateCustomerPaymentCreditCardDataTable(ByVal vInputZipFilename As String,
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentCreditCard")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("CreditCardType", False, SqlDbType.VarChar)
            .Add("CreditCardBankID", False, SqlDbType.UniqueIdentifier)
            .Add("CreditCardBankName", False, SqlDbType.VarChar)
            .Add("IsBankFromOrg", True, SqlDbType.Bit)
            .Add("BankCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("HolderLastName", False, SqlDbType.VarChar)
            .Add("HolderFirstName", False, SqlDbType.VarChar)
            .Add("HolderMiddleName", False, SqlDbType.VarChar)
            .Add("HolderGenerationalSuffix", False, SqlDbType.VarChar)
            .Add("CreditCardNumber", True, SqlDbType.VarChar)
            .Add("CreditCardIssuanceDate", False, SqlDbType.Date)
            .Add("CreditCardExpirationDate", False, SqlDbType.DateTime)
            .Add("ApprovalNo", False, SqlDbType.VarChar)
            .Add("NoOfInstallmentPayments", False, SqlDbType.Int)
            .Add("CreditCardSurcharge", False, SqlDbType.Float)
            .Add("CreditCardAmount", False, SqlDbType.Money)
        End With
        If mConsolidationMode Then
            Return Nothing
        End If
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentCreditCard(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCreditCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvCreditCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        If False Then
            Throw New Exception("Test exception vbAKKJTNVUVM")
        End If

        pSql = "DELETE FROM TransCustomerPaymentCreditCard" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvCreditCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentCreditCard"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If False Then
                    Throw New Exception("Test exception vb9F37BASVPU")
                End If
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    ' additional by janine
    Private Function CreateCustomerPaymentOtherDataTable(ByVal vInputZipFilename As String,
       ByRef oBinder As DictionaryBinder,
       ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentOther")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", False, SqlDbType.UniqueIdentifier)
            .Add("PaymentType", False, SqlDbType.VarChar,, 50)
            .Add("Amount", False, SqlDbType.Money)
            .Add("IssuingOrganization", False, SqlDbType.VarChar,, 255)
            .Add("CardNumber", False, SqlDbType.VarChar,, 30)
            .Add("CardHolderName", False, SqlDbType.VarChar, , 255)
            .Add("CardIssuanceDate", False, SqlDbType.Date)
            .Add("CardExpirationDate", False, SqlDbType.Date)
            .Add("ApprovalNo", False, SqlDbType.VarChar,, 30)
            .Add("GCashRefNo", False, SqlDbType.VarChar,, 50)
        End With
        If mConsolidationMode Then
            Return Nothing
        End If
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentOther(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvDebitCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvDebitCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        pSql = "DELETE FROM TransCustomerPaymentOther" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvDebitCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentOther"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    Private Function CreateCustomerPaymentCheckDataTable(ByVal vInputZipFilename As String,
        ByRef oBinder As DictionaryBinder,
        ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate ' used for verfication only

        pCsvReader = New CsvReader(vInputZipFilename, "TransCustomerPaymentChecks")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("CustomerPaymentID", True, SqlDbType.UniqueIdentifier)
            .Add("BankID", False, SqlDbType.UniqueIdentifier)
            .Add("BankName", False, SqlDbType.VarChar)
            .Add("BankBranchID", False, SqlDbType.UniqueIdentifier)
            .Add("BankBranchName", False, SqlDbType.VarChar)
            .Add("IsBankFromOrg", False, SqlDbType.Bit)
            .Add("AccountName", False, SqlDbType.VarChar)
            .Add("AccountNumber", False, SqlDbType.VarChar)
            .Add("CheckNumber", False, SqlDbType.VarChar)
            .Add("CheckDate", False, SqlDbType.DateTime)
            .Add("CheckAmount", False, SqlDbType.Money)
            .Add("IsDeposit", False, SqlDbType.Bit)
            .Add("ReplacementMemoID", False, SqlDbType.UniqueIdentifier)
            .Add("IsHold", False, SqlDbType.Bit)
            .Add("HoldUntil", False, SqlDbType.DateTime)
            .Add("HoldReason", False, SqlDbType.VarChar)
            .Add("IsReturned", False, SqlDbType.Bit)
            .Add("ReturnedReason", False, SqlDbType.VarChar)
            .Add("DepositDate", False, SqlDbType.DateTime)
            .Add("IsGiftCertificate", False, SqlDbType.Bit)
        End With
        If mConsolidationMode Then
            Return Nothing
        End If
        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do

            If IsDate(pLine("CheckDate")) = False Then
                AppendToDebug("vbM2NSDHB7NU! ImportCustomerPaymentChecks: " & ReadyGuid(pLine("ID")) & " does not have a valid CheckDate")
                pLine("CheckDate") = Now.Date
            End If
            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut

    End Function

    Private Function ImportCustomerPaymentChecks(ByVal vDT As DataTable,
        ByVal vBinder As DictionaryBinder,
        ByVal vCustomerPaymentID As String,
        ByRef rDtOut As DataTable,
        ByRef oError As String) As Boolean

        Dim pDrOut As DataRow
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pDvCreditCard As DataView

        If rDtOut Is Nothing Then
            rDtOut = New DataTable
            rDtOut.Columns.Add("ID")
        End If
        pDvCreditCard = New DataView(vDT, "CustomerPaymentID = " & ReadyString(vCustomerPaymentID), "", DataViewRowState.CurrentRows)

        If False Then
            Throw New Exception("Test exception vbAKKJTNVUCV")
        End If

        pSql = "DELETE FROM TransCustomerPaymentChecks" & vbCrLf &
            " WHERE CustomerPaymentID = " & ReadyString(vCustomerPaymentID)
        mBusiness.ProData.ExecuteSQL(pSql)

        For Each pDrvIn As DataRowView In pDvCreditCard
            pIU = vBinder.CreateInsertUpdate(pDrvIn.Row, oError)
            If pIU Is Nothing Then
                Return False
            End If
            pIU.TableName = "TransCustomerPaymentChecks"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pDrvIn("ID")))
            Try
                If False Then
                    Throw New Exception("Test exception vb9F37BASVZX")
                End If
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = rDtOut.NewRow
                pDrOut("ID") = CStr2(pDrvIn("ID"))
                rDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Next pDrvIn

        Return True

    End Function

    Private Function ImportLibraryOrganization(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String

        DataImportLog.AddLog("Import from LibraryOrganization")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganization")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("IsOrganization", False, SqlDbType.Bit)
            .Add("PersonTitleID", False, SqlDbType.UniqueIdentifier)
            .Add("PersonFullname", False, SqlDbType.VarChar)
            .Add("PersonSurname", False, SqlDbType.VarChar)
            .Add("PersonFirstName", False, SqlDbType.VarChar)
            .Add("PersonMiddleName", False, SqlDbType.VarChar)
            .Add("PersonSuffixName", False, SqlDbType.VarChar)
            .Add("PersonDateOfBirth", False, SqlDbType.Date)
            .Add("PersonMotherMaidenSurname", False, SqlDbType.VarChar)
            .Add("PersonMotherMaidenFirstName", False, SqlDbType.VarChar)
            .Add("PersonMotherMaidenMiddleName", False, SqlDbType.VarChar)
            .Add("PersonSeniorCitizensIDNo", False, SqlDbType.VarChar)
            .Add("PersonPWDIDNo", False, SqlDbType.VarChar)
            .Add("PersonPassportNo", False, SqlDbType.VarChar)
            .Add("PersonDriversLicenseNo", False, SqlDbType.VarChar)
            .Add("PersonSSSNo", False, SqlDbType.VarChar)
            .Add("PersonGSISIDNo", False, SqlDbType.VarChar)
            .Add("PersonPRCIDNo", False, SqlDbType.VarChar)
            .Add("PersonPostalIDNo", False, SqlDbType.VarChar)
            .Add("PersonVotersIDNo", False, SqlDbType.VarChar)
            .Add("PersonOtherIDType", False, SqlDbType.VarChar)
            .Add("PersonOtherIDNo", False, SqlDbType.VarChar)
            .Add("CompanyName", False, SqlDbType.VarChar)
            .Add("CompanyNameInitial", False, SqlDbType.VarChar)
            .Add("OwnershipType", False, SqlDbType.VarChar)
            .Add("IsFranchisor", False, SqlDbType.Bit)
            .Add("RegistrationType", False, SqlDbType.VarChar)
            .Add("RegistrationNo", False, SqlDbType.VarChar)
            .Add("RegistrationDate", False, SqlDbType.Date)
            .Add("TinNumber", False, SqlDbType.VarChar)
            .Add("OfficeAddressBuildingNo", False, SqlDbType.VarChar)
            .Add("OfficeAddressStreet", False, SqlDbType.VarChar)
            .Add("OfficeAddressCityID", False, SqlDbType.UniqueIdentifier)
            .Add("OfficeAddressCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("BusinessAddressBuildingNo", False, SqlDbType.VarChar)
            .Add("BusinessAddressStreet", False, SqlDbType.VarChar)
            .Add("BusinessAddressCityID", False, SqlDbType.UniqueIdentifier)
            .Add("BusinessAddressCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("Purpose", False, SqlDbType.VarChar)
            .Add("BusinessScope", False, SqlDbType.VarChar)
            .Add("BusinessIndustryText", False, SqlDbType.VarChar)
            .Add("NoOfRetailOutlet", False, SqlDbType.Int)
            .Add("NoOfOffice", False, SqlDbType.Int)
            .Add("NoOfWarehouse", False, SqlDbType.Int)
            .Add("NoOfEmployees", False, SqlDbType.Int)
            .Add("IsHasHoldingCompany", False, SqlDbType.Bit)
            .Add("HoldingCompanyID", False, SqlDbType.UniqueIdentifier)
            .Add("IsABank", False, SqlDbType.Bit)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("SalesAgentCode", False, SqlDbType.VarChar)
            .Add("IsFromDevice", False, SqlDbType.Bit)
            .Add("IsMobizFranchisorRequestSent", False, SqlDbType.Bit)
            .Add("IsMobizConfirmedFranchisor", False, SqlDbType.Bit)
            .Add("ChildName", False, SqlDbType.VarChar)
            .Add("ChildBirthdate", False, SqlDbType.VarChar)
            .Add("ChildAge", False, SqlDbType.Int)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            Dim pPersonDateOfBirth As String
            If pLine.TryGetValue("PersonDateOfBirth", pPersonDateOfBirth) Then
                If IsDate(pPersonDateOfBirth) = False Then
                    pLine("PersonDateOfBirth") = "1970/1/1"
                End If
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "MobizLibraryOrganization"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            If ImportLibraryOrganizationCustomer(vInputZipFilename, pLine("ID"), oError) = False Then
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
            If ImportLibraryOrganizationPriceLevel(vInputZipFilename, pLine("ID"), oError) = False Then
                DataImportLog.AddLog(oError)
                DataImportLog.Save(DataImportLog.eResultType.Fail)
                Return False
            End If
        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryOrganization")
        End If

        Return True

    End Function

    Private Function ImportLibraryOrganizationCustomerCreditTerms(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        DataImportLog.AddLog("Import from LibraryOrganizationCustomerCreditTerms")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationCustomerCreditTerms")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("SalesRepID", False, SqlDbType.UniqueIdentifier)
            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier)
            .Add("CreditTermsID", False, SqlDbType.UniqueIdentifier)
            .Add("CreditLimit", False, SqlDbType.Decimal)
            .Add("CreditBegBalance", False, SqlDbType.Money)
            .Add("CreditBalanceAsOf", False, SqlDbType.DateTime)
            .Add("ThirdPartyBegBalance", False, SqlDbType.Money)
            .Add("ThirdPartyBegBalanceAsOf", False, SqlDbType.DateTime)
            .Add("UnlimitedCredit", False, SqlDbType.Bit)
            .Add("CustomerRequirePoNum", False, SqlDbType.Bit)
            .Add("RequireCnCApproval", False, SqlDbType.Bit)
            .Add("EncodedBy", False, SqlDbType.UniqueIdentifier)
            .Add("EncodedDate", False, SqlDbType.DateTime)
            .Add("ApprovedBy", False, SqlDbType.UniqueIdentifier)
            .Add("ApprovedDate", False, SqlDbType.DateTime)
            .Add("UpdatedBy", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", True, SqlDbType.DateTime)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")
        pDtOut.Columns.Add("LastUpdated", GetType(DateTime))

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryOrganizationCustomerCreditTerms"

            pSql = "SELECT LastUpdated" & vbCrLf &
                " FROM " & pIU.TableName & vbCrLf &
                " WHERE ID = " & ReadyString(pLine("ID"))
            Dim pDR = mBusiness.ProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                pSql = pIU.GetInsert
            ElseIf If(CDateNull(pDR("LastUpdated")), New Date(2022, 1, 1)) < If(CDateNull(pLine("LastUpdated")), New Date(2022, 1, 1)) Then
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Else
                pSql = Nothing
            End If

            Try
                If pSql IsNot Nothing Then
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDrOut("LastUpdated") = pLine("LastUpdated")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryOrganizationCustomerCreditTerms")
        End If

        Return True

    End Function

    Private Function ImportLibraryOrganizationCustomer(ByVal vInputZipFilename As String,
        ByVal vOrganizationID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationCustomer")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", True, SqlDbType.UniqueIdentifier)
            .Add("ActivationDate", False, SqlDbType.DateTime)
            .Add("Deactivated", False, SqlDbType.Bit)
            .Add("DeactivationDate", False, SqlDbType.DateTime)
            .Add("DeactivationReason", False, SqlDbType.VarChar)
            .Add("Comments", False, SqlDbType.VarChar)
            .Add("IsBan", False, SqlDbType.Bit)
            .Add("LastUpdated", False, SqlDbType.DateTime)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM LibraryOrganizationCustomer" & vbCrLf &
            " WHERE OrganizationID = " & ReadyString(vOrganizationID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("OrganizationID")) <> ReadyGuid(vOrganizationID) Then
                Continue Do
            End If

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryOrganizationCustomer"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryOrganization'"
                mBusiness.ProData.ExecuteSQL(pSql)

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        Return True

    End Function

    Private Function ImportLibraryOrganizationPriceLevel(ByVal vInputZipFilename As String, ByVal vOrganizationID As String,
    ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationPriceLevel")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", False, SqlDbType.UniqueIdentifier)
            .Add("PriceLevelID", False, SqlDbType.UniqueIdentifier)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        'pSql = "DELETE FROM LibraryOrganizationPriceLevel" & vbCrLf &
        '    " WHERE OrganizationID = " & ReadyString(vOrganizationID)
        'mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("OrganizationID")) <> ReadyGuid(vOrganizationID) Then
                Continue Do
            End If

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryOrganizationPriceLevel"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        Return True

    End Function

    Private Function ImportLibraryOrganizationFOWST(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
                ByVal vOrgID As String, ByVal vUuid As String,
                ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        DataImportLog.AddLog("Import from LibraryOrganizationFOWST")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryOrganizationFOWST")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("OrganizationID", False, SqlDbType.UniqueIdentifier)
            .Add("FOWSTTypeID", False, SqlDbType.UniqueIdentifier)
            .Add("Code", False, SqlDbType.VarChar)
            .Add("Name", False, SqlDbType.VarChar)
            .Add("Description", False, SqlDbType.VarChar)
            .Add("BusinessTradeNameID", False, SqlDbType.UniqueIdentifier)
            .Add("StreetNumber", False, SqlDbType.VarChar)
            .Add("Street", False, SqlDbType.VarChar)
            .Add("FowstCityID", False, SqlDbType.UniqueIdentifier)
            .Add("FowstCountryID", False, SqlDbType.UniqueIdentifier)
            .Add("SubFOWSTTypeID", False, SqlDbType.UniqueIdentifier)
            .Add("PlateNo", False, SqlDbType.VarChar)
            .Add("NoOfWheels", False, SqlDbType.VarChar)
            .Add("BodyColor", False, SqlDbType.VarChar)
            .Add("Weight", False, SqlDbType.Float)
            .Add("Dimension", False, SqlDbType.VarChar)
            .Add("OwnershipType", False, SqlDbType.Bit)
            .Add("ContractStart", False, SqlDbType.Date)
            .Add("ContractEnd", False, SqlDbType.Date)
            .Add("CostPerMonth", False, SqlDbType.Decimal)
            .Add("IsMortgaged", False, SqlDbType.Bit)
            .Add("IsFranchisee", False, SqlDbType.Bit)
            .Add("FranchisorID", False, SqlDbType.UniqueIdentifier)
            .Add("IsFOWSTInsideAMall", False, SqlDbType.Bit)
            .Add("FOWSTMallName", False, SqlDbType.VarChar)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("FulfillmentCenter", False, SqlDbType.UniqueIdentifier)
            .Add("FOWSTNo", False, SqlDbType.Int)
            .Add("AccountID", False, SqlDbType.VarChar)
            .Add("StoreID", False, SqlDbType.VarChar)
            .Add("IsIncludeSpecialReport", False, SqlDbType.Bit)
            .Add("ShopperTrakAccountID", False, SqlDbType.UniqueIdentifier)
            .Add("IsIncludeToShopperTrakReport", False, SqlDbType.Bit)
            .Add("IsFromDevice", False, SqlDbType.Bit)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "MobizLibraryOrganizationFOWST"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try

        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryOrganizationFOWST")
        End If

        Return True

    End Function
    ' end of additional 

    Private Function ImportTransInventoryProductCount(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pBinder2 As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        Dim pDtItems As DataTable
        Dim Total As Double
        Dim TotalBadCount As Double
        Dim TotalBadCountReason As String

        pDtItems = CreateTableForInventoryCountItems(vInputZipFilename, oError)
        If String.IsNullOrWhiteSpace(oError) = False Then
            AppendToLog(oError, , mLogFile)
            oError = JsonDictionary.Serialize("Error", oError)
            Return False
        End If

        DataImportLog.AddLog("Import from TransInventoryProductCount")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInventoryProductCount")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.UniqueIdentifier)
            .Add("ProductID", False, SqlDbType.UniqueIdentifier)
            .Add("InventoryCountDate", False, SqlDbType.VarChar)
            .Add("BatchID", False, SqlDbType.UniqueIdentifier)
            .Add("UserID", False, SqlDbType.VarChar)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pBinder2 = New DictionaryBinder
        With pBinder2.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InventoryProductCountID", True, SqlDbType.UniqueIdentifier)
            .Add("BinID", True, SqlDbType.UniqueIdentifier)
            .Add("IsGoodStock", True, SqlDbType.Bit)
            .Add("ExpirationDate", True, SqlDbType.VarChar)
            .Add("UnitMeasureID", False, SqlDbType.UniqueIdentifier)
            .Add("Quantity", False, SqlDbType.Int)
            .Add("ActualQuantity", False, SqlDbType.Int)
            .Add("BadCountReason", True, SqlDbType.VarChar)
        End With

        Dim dtAdjustmentItems As Date
        Dim IsHasPossibleReconstruction As Boolean

        Do
            IsHasPossibleReconstruction = False
            'mBusiness.ProData.BeginTransaction()
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                If oError = "FowstID must be a GUID.. Value is Others" Then
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)
                    Continue Do
                Else
                    Throw New ApplicationException(oError & ". ref: vbY3ZG3PL42Z!")
                End If
            End If

            If pDtItems Is Nothing Then
                AppendToLog("Cannot find transaction details in ID " & pLine("ID"))
                Continue Do
            End If

            pSql = "Select top 1 LastUpdated from TransInventoryLedger " & vbCrLf &
                    "where " & vbCrLf &
                    "ProductID =  " & ReadyString(pLine("ProductID")) & vbCrLf &
                    "AND FowstID =  " & ReadyString(pLine("FowstID")) & vbCrLf &
                    "AND Lastupdated > " & ReadyString(pLine("InventoryCountDate"))

            dtAdjustmentItems = CDate(mBusiness.ProData.ExecuteScalar(pSql))

            'Check for adjustment
            If (DateDiff(DateInterval.Second, CDate(pLine("InventoryCountDate")), dtAdjustmentItems) > 0) Then
                'IsHasPossibleReconstruction = True
            End If

            pIU.TableName = "TransInventoryProductCount"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)

                    'TransInventoryProductCount details
                    pDtItems.DefaultView.RowFilter = " InventoryProductCountID = " & ReadyString(pLine("ID"))

                    Dim BinID As String
                    Dim ExpirationDate As String
                    Dim IsGoodCount As Boolean

                    'Tota is for Offtake computation
                    Total = 0
                    ExpirationDate = "N/A"
                    BinID = ""

                    If pDtItems.DefaultView.ToTable.Rows.Count > 0 Then
                        For i As Integer = 0 To pDtItems.DefaultView.ToTable.Rows.Count - 1

                            'check if this is the current default "Unnamed Bin"

                            Dim TempBinID As String
                            Dim TempBinName As String

                            'check if this is UnnamedBin
                            pSql = "Select Name from LibraryFowstBin WHERE ID=" & ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID"))
                            TempBinName = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

                            If TempBinName = "Unnamed Bin" Then
                                'check if this is the existing default bin for this fowst-product

                                pSql = "Select BinID from LibraryFowstBinProduct WHERE ProductID = " & ReadyString(pLine("ProductID")) & " AND BINID = " & ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID")) & " AND FOWSTID = " & ReadyString(pLine("FowstID")) & " AND IsDefault=1"
                                TempBinID = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

                                If TempBinID = vbNullString Then
                                    'get default bin in fowstbinproduct table
                                    pSql = "Select BinID from LibraryFowstBinProduct WHERE ProductID = " & ReadyString(pLine("ProductID")) & " AND FOWSTID = " & ReadyString(pLine("FowstID")) & " AND IsDefault=1"
                                    TempBinID = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

                                    If TempBinID = vbNullString Then
                                        'if there is no default bin, this bin id will be tagged as default bin in inventory ledger function
                                        BinID = CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID"))
                                    Else
                                        'use current default bin
                                        BinID = TempBinID
                                    End If
                                Else
                                    'this is the existing default bin
                                    BinID = TempBinID
                                End If
                            Else
                                'this is not unnamed bin
                                BinID = CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID"))
                            End If


                            Dim p As New InsertUpdate("TransInventoryProductCountStock")

                            Dim pTotal As Integer = 0
                            TotalBadCount = 0
                            TotalBadCountReason = ""

                            p("ID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ID"))
                            p("InventoryProductCountID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("InventoryProductCountID"))

                            p("BinID") = ReadyString(BinID)

                            'ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("BinID"))
                            'BinID = pDtItems.DefaultView.ToTable.Rows(i).Item("BinID")

                            p("IsGoodStock") = CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("IsGoodStock"))

                            If Not pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate") = "N/A" Then
                                p("ExpirationDate") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate"))
                                ExpirationDate = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpirationDate"))
                            End If

                            p("UnitMeasureID") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("UnitMeasureID"))
                            p("Quantity") = CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity"))
                            p("ActualQuantity") = CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))

                            p("Variance") = CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("Variance"))
                            p("ExpectedValue") = CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("ExpectedValue"))
                            p("Remarks") = ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("Remarks"))

                            If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")) = vbNullString Then
                                p("TurnOverToID") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")))
                            End If

                            If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")) = vbNullString Then
                                p("TurnOverDate") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")))
                            End If

                            If IsInventoryCountAutomaticEffect() Then
                                p("IsActionTaken") = 1
                                p("IsVarianceApproved") = 1
                            End If

                            pSql = p.GetInsert()

                            If (CBool(pDtItems.DefaultView.ToTable.Rows(i).Item("IsGoodStock"))) Then
                                Total += CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) '* CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))
                                pTotal = CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) '* CInt(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))
                            Else
                                TotalBadCount += CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("Quantity")) * CDec2(pDtItems.DefaultView.ToTable.Rows(i).Item("ActualQuantity"))
                                TotalBadCountReason = CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("BadCountReason"))
                            End If

                            mBusiness.ProData.ExecuteSQL(pSql)

                            If IsInventoryCountAutomaticEffect() Then
                                AddToInventoryLedger(pLine("ID"), BinID, Total, ExpirationDate, pLine("ProductID"), pLine("FowstID"), pLine("UserID"), TotalBadCount, TotalBadCountReason, IsHasPossibleReconstruction, CStr2(pLine("InventoryCountDate")))
                            Else
                                Dim TransactionBalanceBeforeCount As Double

                                pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                                "where LastUpdated < " & ReadyString(CDate(pLine("InventoryCountDate"))) & vbCrLf &
                                "AND TransInventoryLedger.FowstID = " & ReadyString(pLine("FowstID")) & vbCrLf &
                                "AND TransInventoryLedger.BinID = " & ReadyString(BinID) & vbCrLf &
                                "AND TransInventoryLedger.ProductID = " & ReadyString(pLine("ProductID")) & vbCrLf &
                                " ORDER BY LastUpdated desc"

                                TransactionBalanceBeforeCount = CDec2(mBusiness.ProData.ExecuteScalar(pSql))

                                pSql = "Update TransInventoryProductCountStock set Variance = " & Total - TransactionBalanceBeforeCount & vbCrLf &
                                        " WHERE ID = " & ReadyString(pDtItems.DefaultView.ToTable.Rows(i).Item("ID")) & vbCrLf &
                                        " AND BinID = " & ReadyString(BinID)

                                mBusiness.ProData.ExecuteSQL(pSql)

                            End If

                        Next

                        AddOrSetNewDefaultBin(pLine("ProductID"), pLine("FowstID"), pDtItems.DefaultView.ToTable.Rows(0).Item("BinID"), pLine("UserID"))

                        ComputeOffTake(pLine("ID"), pLine("FowstID"), pDtItems.DefaultView.ToTable.Rows(0).Item("BinID"), pLine("ProductID"), Total, pLine("InventoryCountDate"))

                    End If
                Else

                    For i As Integer = 0 To pDtItems.DefaultView.ToTable.Rows.Count - 1
                        Dim p As New InsertUpdate("TransInventoryProductCountStock")

                        If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")) = vbNullString Then
                            p("TurnOverToID") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverToID")))

                            If Not CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")) = vbNullString Then
                                p("TurnOverDate") = ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("TurnOverDate")))
                            End If

                            pSql = p.GetUpdate("ID=" & ReadyString(CStr2(pDtItems.DefaultView.ToTable.Rows(i).Item("ID"))))

                            mBusiness.ProData.ExecuteSQL(pSql)
                        End If

                    Next

                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction(, True)
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            'mBusiness.ProData.CommitTransaction()
        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "TransInventoryProductCount")
        End If

        Return True
    End Function

    Function ComputeOffTake(ByVal InventoryCountID As String, ByVal FOWSTID As String, ByVal BinID As String, ByVal ProductID As String, ByVal TotalCount As Double, ByVal CountDate As Date)
        Dim pSql As String
        Dim dtPreviousBalance As DataTable
        Dim OffTake As Double

        pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                "WHERE ProductID = " & ReadyString(ProductID) & vbCrLf &
                " AND FOWSTID = " & ReadyString(FOWSTID) & vbCrLf &
                " AND BINID = " & ReadyString(BinID) & vbCrLf &
                " order by LastUpdated desc"

        dtPreviousBalance = mBusiness.ProData.Execute(pSql)

        If dtPreviousBalance.Rows.Count > 0 Then
            OffTake = CDbl(dtPreviousBalance.Rows(0).Item("Balance")) - TotalCount
            Dim p As New InsertUpdate("TransOffTake")

            p("OfftakeDate") = ReadyString(CountDate)
            p("InventoryCountID") = ReadyString(InventoryCountID)
            p("ProductID") = ReadyString(ProductID)
            p("FowstID") = ReadyString(FOWSTID)
            p("BinID") = ReadyString(BinID)
            p("OffTake") = ReadyString(OffTake)

            pSql = p.GetInsert

            mBusiness.ProData.ExecuteSQL(pSql)
        End If
    End Function

    Function IsInventoryCountAutomaticEffect() As Boolean
        Return mBusiness.SystemDefaults.IsAutomaticEffect
    End Function

    Private Function ImportLibraryFOWSTBin(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
            ByVal vOrgID As String, ByVal vUuid As String,
            ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pLineNumber As Integer

        DataImportLog.AddLog("Import from LibraryFOWSTBin")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryFOWSTBin")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.VarChar)
            .Add("Name", False, SqlDbType.VarChar)
            .Add("Description", False, SqlDbType.VarChar)
            .Add("IsActive", False, SqlDbType.Bit)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            'mBusiness.ProData.BeginTransaction()
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pLineNumber += 1
            Dim pFowstID As String
            pLine.TryGetValue("FowstID", pFowstID)
            If String.IsNullOrWhiteSpace(pFowstID) = False Then
                If IsGuid(pFowstID) = False Then
                    pLine("FowstID") = Guid.NewGuid.ToString()
                End If
            End If
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryFOWSTBin"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf &
                    "CSV line number: " & pLineNumber & vbCrLf &
                    vbCrLf &
                    pIU.GetUpdate("for debugging only") & vbCrLf &
                    vbCrLf &
                    GetStackTrace(, ex))
                Return False
            End Try
            'mBusiness.ProData.CommitTransaction()
        Loop

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryFOWSTBin")
        End If

        Return True
    End Function

    Private Function ImportLibraryFOWSTBinProduct(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
            ByVal vOrgID As String, ByVal vUuid As String,
            ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pLineNumber As Integer
        Dim pID As String
        Dim pEnableWorkaround As Boolean

        'pEnableWorkaround = Now.Date = New Date(2015, 4, 13)
        pEnableWorkaround = Now.Date <= New Date(2015, 11, 3)
        DataImportLog.AddLog("Import from LibraryFowstBinProduct")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryFowstBinProduct")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("BinID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("ReorderQty", False, SqlDbType.Float)
            .Add("MaxPerProduct", False, SqlDbType.Float)
            .Add("ReorderPoint", False, SqlDbType.Float)
            .Add("IsDefault", False, SqlDbType.Bit)
            .Add("IsActive", False, SqlDbType.Bit)
            .Add("LastUpdated", False, SqlDbType.DateTime)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pLineNumber += 1
            pBinder.Source = pLine

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                If pEnableWorkaround Then
                    pID = ""
                    If pLine.ContainsKey("ID") Then
                        pID = pLine("ID")
                        If IsGuid(pID) Then
                            pDrOut = pDtOut.NewRow
                            pDrOut("ID") = pLine("ID")
                            pDtOut.Rows.Add(pDrOut)
                        End If
                    End If
                End If ' If pEnableWorkaround
                Continue Do
            End If
            pIU.TableName = "LibraryFowstBinProduct"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf &
                    "CSV line number: " & pLineNumber & vbCrLf &
                    vbCrLf &
                    pIU.GetUpdate("for debugging only") & vbCrLf &
                    vbCrLf &
                    GetStackTrace(, ex))
                Return False
            End Try
        Loop

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryFOWSTBinProduct")
        End If

        Return True
    End Function

    Private Function CreateTableForInventoryCountItems(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable
        Dim pCsvReader As CsvReader
        Dim pDtOut As DataTable
        Dim pLine As Dictionary(Of String, String)
        Dim oBinder As DictionaryBinder
        Dim pIU As InsertUpdate ' used for verfication only
        Dim pStr As String

        DataImportLog.AddLog("Import from TransInventoryProductCountStock")
        pCsvReader = New CsvReader(vInputZipFilename, "TransInventoryProductCountStock")
        oBinder = New DictionaryBinder
        With oBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("InventoryProductCountID", True, SqlDbType.UniqueIdentifier)
            ' .Add("BinID", True, SqlDbType.UniqueIdentifier) ' // this is the original code
            .Add("BinID", False, SqlDbType.VarChar)
            .Add("IsGoodStock", True, SqlDbType.Bit)
            .Add("ExpirationDate", True, SqlDbType.VarChar)
            .Add("UnitMeasureID", True, SqlDbType.UniqueIdentifier)
            ' .Add("Quantity", True, SqlDbType.Decimal)
            .Add("Quantity", False, SqlDbType.VarChar)
            .Add("ActualQuantity", True, SqlDbType.Decimal)
            .Add("BadCountReason", False, SqlDbType.VarChar)
            .Add("TurnOverToID", False, SqlDbType.VarChar)
            .Add("TurnOverDate", False, SqlDbType.DateTime2)
            .Add("Variance", False, SqlDbType.Decimal)
            .Add("Remarks", False, SqlDbType.VarChar)
            .Add("ExpectedValue", False, SqlDbType.Decimal)
        End With

        pDtOut = oBinder.CreateDataTable

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do

            If pLine.TryGetValue("BinID", pStr) = False Then
                pStr = Guid.NewGuid().ToString()
            End If
            If IsGuid(pStr) = False Then
                pStr = Guid.NewGuid().ToString()
            End If
            pLine("BinID") = pStr

            If pLine.TryGetValue("Quantity", pStr) = False Then
                Continue Do ' danger code: this will ignore error
            End If
            If IsNumeric(pStr) = False Then
                Continue Do ' danger code: this will ignore error
            End If
            pStr = CDec2(pStr)
            pLine("Quantity") = pStr

            oBinder.Source = pLine
            pIU = oBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Return Nothing
            End If
            pDtOut.Rows.Add(oBinder.CreateDataRow(pDtOut))
        Loop

        Return pDtOut
    End Function

    Function AddToInventoryLedger(ByVal TransactionID As String, ByVal BinID As String, ByVal Total As Double, ByVal ExpDate As String, ByVal ProductID As String, ByVal FOWSTID As String, ByVal PeopleID As String, ByVal BadCount As Integer, ByVal BadCountReason As String, Optional ByVal IsHasPossibleReconstruction As Boolean = False, Optional ByVal dtAdjustmentItems As String = "")
        Dim EDate As Date?

        If Not ExpDate = "N/A" Then
            EDate = CDate(ExpDate)
        End If

        Dim TransactionBalanceBeforeCount As String
        Dim pDataNeedsReconstruction As Boolean
        Dim pSql As String
        Dim dtTransactionAfterCount As DataTable

        If IsHasPossibleReconstruction Then

            Dim pInventoryCountDate As String

            pSql = "Select top 1 LastUpdated from TransInventoryLedger " & vbCrLf &
                    "where " & vbCrLf &
                    "ProductID =  " & ReadyString(ProductID) & vbCrLf &
                    "AND FowstID =  " & ReadyString(FOWSTID) & vbCrLf &
                    "AND BinID =  " & ReadyString(BinID) & vbCrLf &
                    "AND Lastupdated > " & ReadyString(dtAdjustmentItems)

            pInventoryCountDate = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

            If IsDate(pInventoryCountDate) Then
                If (DateDiff(DateInterval.Second, CDate(dtAdjustmentItems), CDate(pInventoryCountDate)) > 0) Then
                    'pDataNeedsReconstruction = True

                    pSql = "Select * from TransInventoryLedger " & vbCrLf &
                            "where LastUpdated >= " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                            "AND TransInventoryLedger.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND TransInventoryLedger.BinID = " & ReadyString(BinID) & vbCrLf &
                            "AND TransInventoryLedger.ProductID = " & ReadyString(ProductID) & vbCrLf &
                            " ORDER BY LastUpdated asc"
                    dtTransactionAfterCount = mBusiness.ProData.Execute(pSql)

                    pSql = "Select top 1 Balance from TransInventoryLedger " & vbCrLf &
                            "where LastUpdated < " & ReadyString(CDate(pInventoryCountDate)) & vbCrLf &
                            "AND TransInventoryLedger.FowstID = " & ReadyString(FOWSTID) & vbCrLf &
                            "AND TransInventoryLedger.BinID = " & ReadyString(BinID) & vbCrLf &
                            "AND TransInventoryLedger.ProductID = " & ReadyString(ProductID) & vbCrLf &
                            " ORDER BY LastUpdated desc"

                    TransactionBalanceBeforeCount = mBusiness.ProData.ExecuteScalar(pSql)

                End If
            End If
        End If

        If pDataNeedsReconstruction Then
            mBusiness.TransInventoryLedger.Add(
            vStockID:=CStr2(ProductID),
            vFowstID:=CStr2(FOWSTID),
            vTransactionID:=TransactionID,
            vTransactionType:=TransInventoryLedger.eTransType.InventoryCount,
            vUserID:=CStr2(PeopleID),
            vQty:=Total - TransactionBalanceBeforeCount,
            vBalance:=Total
        )
            If dtTransactionAfterCount.Rows.Count > 0 Then
                For i As Integer = 0 To dtTransactionAfterCount.Rows.Count - 1
                    Dim mTransType As TransInventoryLedger.eTransType

                    Select Case CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionType"))
                        Case "Invoice"
                            mTransType = TransInventoryLedger.eTransType.Invoice
                        Case "InventoryCount"
                            mTransType = TransInventoryLedger.eTransType.InventoryCount
                        Case "Receiving"
                            mTransType = TransInventoryLedger.eTransType.Receiving
                        Case "TransferOut"
                            mTransType = TransInventoryLedger.eTransType.TransferOut
                        Case "Sales Returns"
                            mTransType = TransInventoryLedger.eTransType.SalesReturn
                        Case "Product Maker"
                            mTransType = TransInventoryLedger.eTransType.SalesReturn
                    End Select

                    If mTransType <> TransInventoryLedger.eTransType.InventoryCount Then
                        mBusiness.TransInventoryLedger.Add(
                        vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                        vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                        vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                        vTransactionType:=mTransType,
                        vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                        vQty:=CDec2(dtTransactionAfterCount.Rows(i).Item("Qty"))
                    )
                    Else
                        mBusiness.TransInventoryLedger.Add(
                        vStockID:=CStr2(dtTransactionAfterCount.Rows(i).Item("ProductID")),
                        vFowstID:=CStr2(dtTransactionAfterCount.Rows(i).Item("FowstID")),
                        vTransactionID:=CStr2(dtTransactionAfterCount.Rows(i).Item("TransactionID")),
                        vTransactionType:=mTransType,
                        vUserID:=CStr2(dtTransactionAfterCount.Rows(i).Item("UserID")),
                        vQty:=Nothing,
                        vBalance:=CDec2(dtTransactionAfterCount.Rows(i).Item("Balance"))
                    )
                    End If

                Next
            End If
        Else
            'AppendToDebug("mBusiness.TransInventoryLedger.Add") ' commented 2024_04_26__09_35_32_2426

            'AppendToDebug(ProductID & " " & vbCrLf &
            'FOWSTID & " " & vbCrLf &
            'TransactionID & " " & vbCrLf &
            'PeopleID & " " & vbCrLf &
            'Total & " " & vbCrLf &
            'BinID & " " & vbCrLf &
            'EDate & " " & vbCrLf &
            'BadCount & " " & vbCrLf &
            'BadCountReason) ' commented 2024_04_26__09_35_32_2426

            mBusiness.EnableExtraLogs = True

            mBusiness.TransInventoryLedger.Add(
            ProductID,
            FOWSTID,
            TransactionID,
            TransInventoryLedger.eTransType.InventoryCount,
            PeopleID, ,
            Total,
            BinID,
            EDate,
            BadCount,
            BadCountReason
        )

            'AppendToDebug("end of mBusiness.TransInventoryLedger.Add")
        End If


    End Function

    Function AddOrSetNewDefaultBin(ByVal ProductID As String, ByVal FowstID As String, ByVal BinID As String, ByVal UserID As String)
        'this function should only be applied in inventory count
        Dim pSql As String
        Dim dt As New DataTable
        Dim pOut As String
        Dim currentBinName As String

        pSql = "Select Name from LibraryFowstBin where ID  = " & ReadyString(BinID)

        currentBinName = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

        If Not currentBinName.ToLower = "unnamed bin" Then

            'for carry over
            pSql = "SELECT TOP 1 Balance" & vbCrLf &
                    " FROM TransInventoryLedger " & vbCrLf &
                    " INNER JOIN dbo.LibraryFowstBin ON " & vbCrLf &
                    " dbo.LibraryFowstBin.ID = TransInventoryLedger.BinID " & vbCrLf &
                    " WHERE ProductID = " & ReadyString(ProductID) & vbCrLf &
                    " AND TransInventoryLedger.FowstID = " & ReadyString(FowstID) & vbCrLf &
                    " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

            'If vExpirationDate IsNot Nothing Then
            '    pSql &= " AND ExpirationDate = " & ReadyString(vExpirationDate) & vbCrLf
            'Else
            '    pSql &= " AND ExpirationDate IS NULL" & vbCrLf
            'End If

            pSql &= " ORDER BY TransInventoryLedger.LastUpdated DESC"

            Dim Balance As Double

            Balance = CDec2(mBusiness.ProData.ExecuteScalar(pSql))

            'Query if the bin has fowstbinproduct record()
            pSql = "SELECT BinID" &
             " FROM LibraryFowstBinProduct" &
             " WHERE FowstID = " & ReadyString(FowstID) &
             " AND ProductID = " & ReadyString(ProductID) &
             " AND BinID = " & ReadyString(BinID)
            pOut = CStr2(mBusiness.ProData.ExecuteScalar(pSql))


            pSql = "SELECT dbo.LibraryFowstBin.ID " &
                    "FROM dbo.LibraryFowstBin INNER JOIN " &
                    "dbo.LibraryFowstBinProduct ON dbo.LibraryFowstBin.FowstID = dbo.LibraryFowstBinProduct.FowstID AND  " &
                    "dbo.LibraryFowstBin.ID = dbo.LibraryFowstBinProduct.BinID " &
                    "WHERE dbo.LibraryFowstBinProduct.ProductID = " & ReadyString(ProductID) &
                    " AND dbo.LibraryFowstBinProduct.IsDefault = 1 " &
                    " AND  dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID) &
                    " AND dbo.LibraryFowstBin.Name = 'Unnamed Bin'"

            dt = mBusiness.ProData.Execute(pSql)

            If dt.Rows.Count > 0 Then
                'set default bin balance to 0
                Dim nID As String

                nID = Guid.NewGuid.ToString

                'AddToInventoryLedger(nID, dt.Rows(0).Item("ID").ToString, 0, "N/A", ProductID, FowstID, UserID, 0, "")

                mBusiness.TransInventoryLedger.Add(
                                       ProductID,
                                       FowstID,
                                       nID,
                                       TransInventoryLedger.eTransType.InventoryCountAdjustment,
                                       CStr2(UserID),
                                       , 0,
                                       , ,
                                       0,
                                       ""
                                   )
                'mBusiness.TransInventoryLedger.Add( _
                '      vStockID:=CStr2(ProductID),
                '      vFowstID:=CStr2(FowstID),
                '      vTransactionID:=nID,
                '      vBinID:=dt.Rows(0).Item("ID").ToString,
                '      vQty:=0,
                '      vTransactionType:=TransInventoryLedger.eTransType.InventoryCountAdjustment,
                '      vUserID:=CStr2(UserID) _
                '  )

                'remove isdefault to "Default Bin"
                pSql = "Update LibraryFowstBinProduct set IsDefault = 0 where ProductID = " & ReadyString(ProductID) & " AND BinID = " & ReadyString(dt.Rows(0).Item("ID").ToString) & " AND dbo.LibraryFowstBinProduct.FowstID = " & ReadyString(FowstID)
                mBusiness.ProData.ExecuteSQL(pSql)

                'carry over

                'AddToInventoryLedger(nID, dt.Rows(0).Item("ID").ToString, Balance, "N/A", ProductID, FowstID, UserID, 0, "")

                mBusiness.TransInventoryLedger.Add(
                       vStockID:=CStr2(ProductID),
                       vFowstID:=CStr2(FowstID),
                       vTransactionID:=nID,
                       vTransactionType:=TransInventoryLedger.eTransType.CarryOver,
                       vUserID:=CStr2(UserID),
                       vQty:=Balance,
                       vBinID:=BinID
                   )


                'add to libraryfowstbinproduct and set as default bin
                Dim pIU As InsertUpdate

                pIU = New InsertUpdate("LibraryFowstBinProduct")
                pIU("FowstID") = ReadyString(FowstID)
                pIU("BinID") = ReadyString(BinID)
                pIU("ProductID") = ReadyString(ProductID)
                pIU("IsDefault") = 1
                pSql = pIU.GetInsert
                mBusiness.ProData.ExecuteSQL(pSql)

                pOut = "Not Empty"
            End If

            'check if the current bin is already assigned to this product in LibraryFowstBinProduct

            If CStr2(pOut) = "" Then
                Dim pIU As InsertUpdate

                pIU = New InsertUpdate("LibraryFowstBinProduct")
                pIU("FowstID") = ReadyString(FowstID)
                pIU("BinID") = ReadyString(BinID)
                pIU("ProductID") = ReadyString(ProductID)
                pIU("IsDefault") = 0
                pSql = pIU.GetInsert
                mBusiness.ProData.ExecuteSQL(pSql)
            End If
        End If
    End Function

    Private Function ImportLibraryProductsWithoutBarcode(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
            ByVal vOrgID As String, ByVal vUuid As String,
            ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pLineNumber As Integer
        Dim pID As String
        Dim pEnableWorkaround As Boolean

        DataImportLog.AddLog("Import from LibraryProductsWithoutBarcode")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryProductsWithoutBarcode")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pLineNumber += 1
            pBinder.Source = pLine

            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                If pEnableWorkaround Then
                    pID = ""
                    If pLine.ContainsKey("ID") Then
                        pID = pLine("ID")
                        If IsGuid(pID) Then
                            pDrOut = pDtOut.NewRow
                            pDrOut("ID") = pLine("ID")
                            pDtOut.Rows.Add(pDrOut)
                        End If
                    End If
                End If ' If pEnableWorkaround
                Continue Do
            End If
            pIU.TableName = "LibraryProductsWithoutBarcode"
            pSql = pIU.GetUpdate("ProductID = " & ReadyString(pLine("ProductID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)

                    Dim mCount As Double
                    Dim IsExist As Boolean

                    pSql = "Update SystemDefaults set DefaultValue = convert(int,DefaultValue) + 1 where Description = 'Missing Barcode Count'"

                    If Not CBool2(mBusiness.ProData.ExecuteSQL(pSql)) Then
                        pSql = "Insert into SystemDefaults(Description, DefaultValue) values('Missing Barcode Count','1')"
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If

                End If

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = JsonDictionary.Serialize("Error", "An error occured while inserting data: " & ex.Message & ";" & vbCrLf &
                    "CSV line number: " & pLineNumber & vbCrLf &
                    vbCrLf &
                    pIU.GetUpdate("for debugging only") & vbCrLf &
                    vbCrLf &
                    GetStackTrace(, ex))
                Return False
            End Try
        Loop

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryProductsWithoutBarcode")
        End If

        Return True

    End Function

    Private Function ImportReceiving(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView
            Dim pError As String
            Dim pIsInsert As Boolean

            oError = ""

            DataImportLog.AddLog("Import from TransReceiving")
            pCsvReader = New CsvReader(vInputZipFilename, "TransReceiving")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("ReceivingNo", True, SqlDbType.Int)
                .Add("IsReceiving", True, SqlDbType.Bit)
                .Add("ReceivingPlaceID", False, SqlDbType.UniqueIdentifier)
                .Add("ReceiveFromOutlet", False, SqlDbType.UniqueIdentifier)
                .Add("SupplierID", False, SqlDbType.UniqueIdentifier)
                .Add("DocumentDate", True, SqlDbType.DateTime)
                .Add("DocumentType", False, SqlDbType.VarChar)
                .Add("DocumentNo", False, SqlDbType.VarChar)
                .Add("Is3PL", False, SqlDbType.Bit)
                .Add("IsResaleable", False, SqlDbType.Bit)
                .Add("ResaleableReason", False, SqlDbType.VarChar)
                .Add("UserID", True, SqlDbType.UniqueIdentifier)
                .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
                .Add("TransportedBy", False, SqlDbType.VarChar)
                .Add("TransferOutToReceivingID", False, SqlDbType.VarChar) ' should be UniqueIdentifier
                .Add("LastUpdated", False, SqlDbType.VarChar)
                .Add("HasIssues", False, SqlDbType.Bit)
                .Add("IssueRemarks", False, SqlDbType.VarChar)
                .Add("PONo", False, SqlDbType.VarChar)
                .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
                .Add("IsRefund", False, SqlDbType.Bit)
                .Add("ReceiveInDepartment", False, SqlDbType.UniqueIdentifier)
                .Add("ReceiveFromDepartment", False, SqlDbType.UniqueIdentifier)
                .Add("DeviceFowstTransactionNumber", False, SqlDbType.Int)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Dim dtAdjustmentItems As Date
            Dim IsHasPossibleReconstruction As Boolean

            Do
                IsHasPossibleReconstruction = False
                mBusiness.ProData.RollbackTransaction(True, True)
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                If pBinder.Validate(oError) = False Then
                    Return False
                End If
                If pItemInitialized = False Then
                    pItemInitialized = True
                    pDtItems = CreateReceivingStocksDataTable(vInputZipFilename, oError)
                    If String.IsNullOrWhiteSpace(oError) = False Then
                        Return False
                    End If
                End If
                If pLine.ContainsKey("ReceivingPlaceID") = False _
                    OrElse String.IsNullOrWhiteSpace(CStr2(pLine("ReceivingPlaceID"))) Then

                    If mFowstID Is Nothing Then
                        mFowstID = mBusiness.TransReceiving.CommonReceivingPlaceID(pLine("DeviceID"))
                        If mFowstID Is Nothing Then
                            oError = "ReceivingPlaceID must not be blank. vb6W8YG7G89G!"
                            Return False
                        End If
                    Else
                        pLine("ReceivingPlaceID") = mFowstID
                        AppendToLog("ReceivingPlaceID is blank so it will be assigned a value from invoice fowst ID. vbPSGRCDJGAM!" & vbCrLf &
                            JsonConvert.SerializeObject(pLine, Formatting.Indented), ,
                            "ImportReceiving FGW7XMCJ7Y!")
                    End If
                End If
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                If pBinder.Items.ContainsKey("SupplierID") = False Then
                    If ReadyGuid(OrgID) = ReadyGuid("9DA344F3-DADB-4675-B723-15F0C591DA3C") Then
                        pIU("SupplierID") = "'f1441fc7-fb7d-47fc-a62b-1548f066fc98'"
                    End If
                End If

                If pBinder.Items.ContainsKey("TransportedBy") = False Then
                    pIU("TransportedBy") = "'Unknown'"
                End If

                If pLine.ContainsKey("TransferOutToReceivingID") _
                    AndAlso String.IsNullOrWhiteSpace(CStr2(pLine("TransferOutToReceivingID"))) = False Then
                    If IsGuid(pLine("TransferOutToReceivingID")) = False Then
                        ' issue with nike abreeza
                        ' receiving id: 18DE9887-3D29-4347-9B9A-090CDCA4B8E3
                        ' TransferOutToReceivingID = 19578262
                        AppendToLog("vb8R95DHKEDB!" & vbCrLf &
                            "zip filename: " & vInputZipFilename & vbCrLf &
                            "receiving id: " & pLine("ID") & vbCrLf &
                            "TransferOutToReceivingID: " & pLine("TransferOutToReceivingID") & vbCrLf, , "ImportReceiving")
                        If ReadyGuid(pLine("ID")) = ReadyGuid("18DE9887-3D29-4347-9B9A-090CDCA4B8E3") Then
                            AppendToDebug("vb9BFLE7HD3R! Tagging as imported. receiving id: 18DE9887-3D29-4347-9B9A-090CDCA4B8E3")
                            pDrOut = pDtOut.NewRow
                            pDrOut("ID") = pLine("ID")
                            pDtOut.Rows.Add(pDrOut)
                        End If
                        Continue Do
                    End If
                Else
                    pIU("TransferOutToReceivingID") = "null"
                End If

                If (CBool2(pIU("IsReceiving")) = False) Then
                    pIU("HasIssues") = 1
                    pIU("IssueRemarks") = ReadyString("No stocks received in destination")
                End If

                pIU.TableName = "TransReceiving"

                dtAdjustmentItems = InventoryAdjustment(pLine("ReceivingPlaceID"))

                'Check for adjustment
                If (DateDiff(DateInterval.Second, CDate(pLine("LastUpdated")), dtAdjustmentItems) > 0) Then
                    'IsHasPossibleReconstruction = True
                End If

                mBusiness.ProData.BeginTransaction()
                Try
                    pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pIsInsert = True
                        pSql = pIU.GetInsert
                        Try
                            mBusiness.ProData.ExecuteSQL(pSql)
                        Catch ex As Exception
                            AppendToLog("vbKX97K6SVLY! " & ex.Message & vbCrLf & pIU.GetUpdate("insert statement"), , mLogFile)
                            Throw
                        End Try
                    Else
                        pIsInsert = False
                    End If

                    Dim pDtItemsTemp As DataTable

                    If pLine.ContainsKey("TransferOutToReceivingID") _
                        AndAlso String.IsNullOrWhiteSpace(CStr2(pLine("TransferOutToReceivingID"))) = False Then
                        pSql = "Update TransReceiving set TransferOutToReceivingID = " & ReadyString(pLine("ID")) &
                            ", HasIssues = 0 " &
                            ", IssueRemarks = '' " &
                            " WHERE ID = " & ReadyString(pLine("TransferOutToReceivingID"))

                        mBusiness.ProData.ExecuteSQL(pSql)

                        pSql = "Select * from TransReceivingStock where ReceivingID = " & ReadyString(pLine("TransferOutToReceivingID"))
                        pDtItemsTemp = mBusiness.ProData.Execute(pSql)
                    End If

                    pSql = "DELETE FROM TransReceivingStock WHERE ReceivingID = " & ReadyString(pLine("ID"))
                    mBusiness.ProData.ExecuteSQL(pSql)

                    ' BEGIN insert items
                    pDvItems = New DataView(pDtItems, "ReceivingID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows)
                    For Each pDrvItem As DataRowView In pDvItems
                        pIU = New InsertUpdate("TransReceivingStock")
                        pIU("ID") = ReadyString(pDrvItem("ID"))
                        pIU("ReceivingID") = ReadyString(pDrvItem("ReceivingID"))
                        pIU("ProductID") = ReadyString(pDrvItem("ProductID"))

                        Dim pQtyInBaseUnit = CDec2(pDrvItem("QuantityGood")) * CDec2(pDrvItem("ActualQuantityGood"))
                        If pQtyInBaseUnit > 10000000 Then
                            AppendToLog("ImportDataClass.ImportReceiving vbMSSERZDXBC! pQtyInBaseUnit=" & pQtyInBaseUnit & vbCrLf &
                                "ProductID=" & CStr2(pDrvItem("ProductID")) & vbCrLf &
                                "QuantityGood=" & CDec2(pDrvItem("QuantityGood")) & vbCrLf &
                                "ActualQuantityGood=" & CDec2(pDrvItem("ActualQuantityGood")))
                            If CDec2(pDrvItem("QuantityGood")) > CDec2(pDrvItem("ActualQuantityGood")) Then
                                pDrvItem("QuantityGood") = 1
                            Else
                                pDrvItem("ActualQuantityGood") = 1
                            End If
                        End If

                        pIU("QuantityGood") = CDec2(pDrvItem("QuantityGood"))
                        If CDec2(pDrvItem("ActualQuantityGood")) = 5184000000 Then
                            AppendToLog("vbHDJR2GJ59J! pDrvItem(ActualQuantityGood) = " & CDec2(pDrvItem("ActualQuantityGood")))
                            pDrvItem("ActualQuantityGood") = 1
                        End If
                        pIU("ActualQuantityGood") = CDec2(pDrvItem("ActualQuantityGood"))
                        pIU("UnitMeasureGood") = ReadyString(pDrvItem("UnitMeasureGood"))
                        pIU("QuantityDamaged") = CDec2(pDrvItem("QuantityDamaged"))
                        If CDec2(pDrvItem("ActualQuantityDamaged")) = 5184000000 Then
                            AppendToLog("vbHDJR2GJ59J! pDrvItem(ActualQuantityDamaged) = " & CDec2(pDrvItem("ActualQuantityDamaged")))
                            pDrvItem("ActualQuantityDamaged") = 1
                        End If
                        pIU("ActualQuantityDamaged") = CDec2(pDrvItem("ActualQuantityDamaged"))
                        pIU("UnitMeasureDamaged") = ReadyString(pDrvItem("UnitMeasureDamaged"))
                        pIU("BadOrderReason") = ReadyString(pDrvItem("BadOrderReason"))
                        pIU("AdditionalInfo") = ReadyString(pDrvItem("AdditionalInfo"))
                        pIU("IsResaleable") = CDec2(pDrvItem("IsResaleable"))
                        pIU("IsShortLanded") = CDec2(pDrvItem("IsShortLanded"))
                        pIU("IsOverLanded") = CDec2(pDrvItem("IsOverLanded"))

                        pSql = pIU.GetInsert()
                        Try
                            mBusiness.ProData.ExecuteSQL(pSql)
                        Catch ex As Exception
                            pError = "vbY4UYVJTN3A!: " & ex.Message & ";" & vbCrLf &
                                "Actual sql: " & pSql & vbCrLf &
                                "UPDATE-converted sql: " & pIU.GetUpdate("INSERT") & vbCrLf &
                                GetStackTrace(, ex)
                            AppendToLog(pError, , mLogFile & " vbY4UYVJTN3A")
                            Throw
                        End Try

                        'If pDtItemsTemp IsNot Nothing AndAlso
                        '    pDtItemsTemp.Rows.Count > 0 Then

                        '    pDtItemsTemp.DefaultView.RowFilter = "ProductID = " & ReadyString(pDrvItem("ProductID"))

                        '    If pDtItemsTemp.DefaultView.ToTable.Rows.Count > 0 Then
                        '        If Not pDtItemsTemp.DefaultView.ToTable.Rows(0).Item("QuantityGood") = pDrvItem("QuantityGood") OrElse _
                        '            Not CStr2(pDtItemsTemp.DefaultView.ToTable.Rows(0).Item("UnitMeasureGood")) = CStr2(pDrvItem("UnitMeasureGood")) Then

                        '            'there's a problem with this sku
                        '            pSql = "Update TransReceiving set HasIssues = 1, " & _
                        '                "IssueRemarks = 'Some discrepancies found'" & _
                        '                " WHERE ID = " & ReadyString(pDrvItem("ReceivingID"))

                        '            mBusiness.ProData.ExecuteSQL(pSql)

                        '            pSql = "Update TransReceiving set HasIssues = 1, " & _
                        '                    "IssueRemarks = 'Some discrepancies found'" & _
                        '                    " WHERE ID = " & ReadyString(pLine("TransferOutToReceivingID"))

                        '            mBusiness.ProData.ExecuteSQL(pSql)

                        '        End If
                        '    Else
                        '        'received item with no stock transfer counterpart?
                        '        pSql = "Update TransReceiving set HasIssues = 1, " &
                        '                "IssueRemarks = 'Some discrepancies found'" & _
                        '                " WHERE ID = " & ReadyString(pDrvItem("ReceivingID"))

                        '        mBusiness.ProData.ExecuteSQL(pSql)

                        '        pSql = "Update TransReceiving set HasIssues = 1, " & _
                        '                "IssueRemarks = 'Some discrepancies found'" & _
                        '                " WHERE ID = " & ReadyString(pLine("TransferOutToReceivingID"))

                        '        mBusiness.ProData.ExecuteSQL(pSql)

                        '    End If
                        'Else
                        '    'received stock transfer counterpart?
                        '    pSql = "Update TransReceiving set HasIssues = 1, " &
                        '        "IssueRemarks = 'No stock transfer document from source'" & _
                        '        " WHERE ID = " & ReadyString(pDrvItem("ReceivingID"))

                        '    mBusiness.ProData.ExecuteSQL(pSql)
                        'End If
                    Next pDrvItem

                    ' END insert items
                    If ImportReceivingStockSerials(vInputZipFilename, pLine("ID"), oError) = False Then
                        DataImportLog.AddLog(oError)
                        DataImportLog.Save(DataImportLog.eResultType.Fail)
                        mBusiness.ProData.RollbackTransaction(True, True)
                        Exit Do
                    End If

                    If pIsInsert Then
                        Try
                            mBusiness.TransReceiving.AddToInventoryLedger(pLine("ID"), IsHasPossibleReconstruction, vOrgID:=vOrgID)
                        Catch ex As ProSqlException
                            Throw New ApplicationException("vbWN4T5MGE6C! " & ex.Message & vbCrLf & ex.Sql, ex)
                        Catch ex As Exception
                            Throw New ApplicationException("vbJGX8R3PH6K! " & ex.Message, ex)
                        End Try
                        mBusiness.TransReceiving.UpdateCounter(
                            vDeviceID:=pLine("DeviceID"),
                            vFowstID:=pLine("ReceivingPlaceID"),
                            vValue:=CDec2(pLine("ReceivingNo")),
                            vIsReceiving:=CBool2(pLine("IsReceiving")))
                    End If

                    mBusiness.ProData.CommitTransaction()

                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)
                Catch ex As Exception
                    AppendToLog(GetFilename() & vbCrLf & "ref: vbJZWJKZS2X2!", , mLogFile)
                    mBusiness.ProData.RollbackTransaction(, True)
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Loop

            If pDtOut.Rows.Count = 0 Then
                If CStr2(oError).Trim <> "" Then
                    Return False
                End If
            Else
                mBusiness.TransBackOrders.CreateOrders()
            End If

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransReceiving")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportReceivingStockSerials(ByVal vInputZipFilename As String, ByVal vReceivingID As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow

        oError = ""

        DataImportLog.AddLog("Import from TransReceivingStockSerials")
        pCsvReader = New CsvReader(vInputZipFilename, "TransReceivingStockSerials")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("SerialNumber", True, SqlDbType.VarChar)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("ReceivingID", True, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        pSql = "DELETE FROM TransReceivingStockSerials" & vbCrLf &
            " WHERE ReceivingID = " & ReadyString(vReceivingID)
        mBusiness.ProData.ExecuteSQL(pSql)

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If ReadyGuid(pLine("ReceivingID")) <> ReadyGuid(vReceivingID) Then
                Continue Do
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransReceivingStockSerials"
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        Return True

    End Function

    Private Function CreateReceivingStocksDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        oError = Nothing
        DataImportLog.AddLog("Import from TransReceivingStock")
        pCsvReader = New CsvReader(vInputZipFilename, "TransReceivingStock")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ReceivingID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("QuantityGood", True, SqlDbType.Decimal)
            .Add("ActualQuantityGood", True, SqlDbType.Decimal)
            .Add("UnitMeasureGood", True, SqlDbType.UniqueIdentifier)
            .Add("QuantityDamaged", True, SqlDbType.Decimal)
            .Add("ActualQuantityDamaged", True, SqlDbType.Decimal)
            .Add("UnitMeasureDamaged", True, SqlDbType.UniqueIdentifier)
            .Add("BadOrderReason", False, SqlDbType.VarChar)
            .Add("AdditionalInfo", False, SqlDbType.VarChar)
            .Add("IsResaleable", True, SqlDbType.Bit)
            .Add("IsShortLanded", False, SqlDbType.Bit)
            .Add("IsOverLanded", False, SqlDbType.Bit)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("ReceivingID")
            .Add("ProductID")
            .Add("QuantityGood", GetType(Decimal))
            .Add("ActualQuantityGood", GetType(Decimal))
            .Add("UnitMeasureGood")
            .Add("QuantityDamaged", GetType(Decimal))
            .Add("ActualQuantityDamaged", GetType(Decimal))
            .Add("UnitMeasureDamaged")
            .Add("BadOrderReason")
            .Add("AdditionalInfo")
            .Add("IsResaleable", GetType(Boolean))
            .Add("IsShortLanded", GetType(Boolean))
            .Add("IsOverLanded", GetType(Boolean))
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)
            pDtOut.Rows.Add(pDR)
        Loop

        Return pDtOut

    End Function

    Private Function ImportProductBarcode(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from LibraryPIMProductMeasurementBarcode")
            pCsvReader = New CsvReader(vInputZipFilename, "LibraryPIMProductMeasurementBarcode")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("StockMeasurementsID", True, SqlDbType.UniqueIdentifier)
                .Add("Barcode", True, SqlDbType.VarChar)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "MobizLibraryPIMProductMeasurementBarcode"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "LibraryPIMProductMeasurementBarcode")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportCashHandling(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from TransCashHandling")
            pCsvReader = New CsvReader(vInputZipFilename, "TransCashHandling")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("FOWSTID", True, SqlDbType.UniqueIdentifier)
                .Add("DepositDate", True, SqlDbType.DateTime2)
                .Add("DepositedToPosition", False, SqlDbType.VarChar)
                .Add("DepositedTo", False, SqlDbType.UniqueIdentifier)
                .Add("DepositedToByName", False, SqlDbType.VarChar)
                .Add("DepositedBy", False, SqlDbType.UniqueIdentifier)
                .Add("DepositedByName", False, SqlDbType.VarChar)
                .Add("IsFromTotalAmount", False, SqlDbType.VarChar)
                .Add("TotalAmount", True, SqlDbType.Decimal)

                .Add("TotalCash", False, SqlDbType.Decimal)
                .Add("TotalCheck", False, SqlDbType.Decimal)
                .Add("TotalCreditCard", False, SqlDbType.Decimal)
                .Add("TotalDebitCard", False, SqlDbType.Decimal)
                .Add("TotalSRV", False, SqlDbType.Decimal)
                .Add("TotalPettyCash", False, SqlDbType.Decimal)

                '.Add("TotalAlipay", False, SqlDbType.Decimal)
                '.Add("TotalComplimentary", False, SqlDbType.Decimal)
                '.Add("TotalDiscountVoucher", False, SqlDbType.Decimal)
                '.Add("TotalEPlus", False, SqlDbType.Decimal)
                '.Add("TotalFoodPanda", False, SqlDbType.Decimal)
                '.Add("TotalGCash", False, SqlDbType.Decimal)
                '.Add("TotalGoldCard", False, SqlDbType.Decimal)
                '.Add("TotalGrabFood", False, SqlDbType.Decimal)
                '.Add("TotalLocalDelivery", False, SqlDbType.Decimal)
                '.Add("TotalPayMaya", False, SqlDbType.Decimal)
                '.Add("TotalShopeeEVoucher", False, SqlDbType.Decimal)
                '.Add("TotalShopeePay", False, SqlDbType.Decimal)
                '.Add("TotalSMAC", False, SqlDbType.Decimal)
                '.Add("TotalVPlus", False, SqlDbType.Decimal)

                'get all other payment type
                pSql = "Select Value from MobizLibraryOtherPayments"
                Dim pDTOtherPaymentType As DataTable

                pDTOtherPaymentType = mBusiness.ProData.Execute(pSql)

                For i As Integer = 0 To pDTOtherPaymentType.Rows.Count - 1
                    .Add("Total" & CStr2(pDTOtherPaymentType.Rows(i).Item("Value")), False, SqlDbType.Decimal)
                Next

                .Add("NoOfOneCents", False, SqlDbType.Decimal)
                .Add("NoOfFiveCents", False, SqlDbType.Decimal)
                .Add("NoOfTenCents", False, SqlDbType.Decimal)
                .Add("NoOfTwentyFiveCents", False, SqlDbType.Decimal)
                .Add("NoOfOnePeso", False, SqlDbType.Decimal)
                .Add("NoOfFivePesos", False, SqlDbType.Decimal)
                .Add("NoOfTenPesos", False, SqlDbType.Decimal)
                .Add("NoOfTwentyPesos", False, SqlDbType.Decimal)
                .Add("NoOfFiftyPesos", False, SqlDbType.Decimal)
                .Add("NoOfOneHundredPesos", False, SqlDbType.Decimal)
                .Add("NoOfTwoHundredPesos", False, SqlDbType.Decimal)
                .Add("NoOfFiveHundredPesos", False, SqlDbType.Decimal)
                .Add("NoOfOneThousandPesos", False, SqlDbType.Decimal)

                .Add("TotalAcceptedFromPrevTurnOver", False, SqlDbType.Decimal)
                .Add("TransactionDateAndTimeFrom", False, SqlDbType.DateTime2)
                .Add("TransactionDateAndTimeTo", False, SqlDbType.DateTime2)
                .Add("IsTurnOver", False, SqlDbType.Bit)

                .Add("ServiceCharge", False, SqlDbType.Decimal)
                .Add("BankBranch", False, SqlDbType.VarChar)
                .Add("DepositReferenceNo", False, SqlDbType.VarChar)
                .Add("LastUpdated", False, SqlDbType.DateTime2)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                If pLine.ContainsKey("LastUpdated") = False Then
                    pLine.Add("LastUpdated", Date.Now())
                End If
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    AppendToDebug(oError)
                    Continue Do
                End If
                pIU.TableName = "TransCashHandling"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransCashHandling")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportCashHandlingCashCount(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from TransCashHandlingCashCount")
            pCsvReader = New CsvReader(vInputZipFilename, "TransCashHandlingCashCount")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("FOWSTID", True, SqlDbType.UniqueIdentifier)
                .Add("CashCountDate", True, SqlDbType.DateTime2)
                .Add("CashCountedBy", True, SqlDbType.UniqueIdentifier)
                .Add("CashCountedByName", False, SqlDbType.VarChar)
                .Add("IsFromTotalAmount", False, SqlDbType.VarChar)
                .Add("TotalAmount", True, SqlDbType.Decimal)

                .Add("TotalCash", False, SqlDbType.Decimal)
                .Add("TotalCheck", False, SqlDbType.Decimal)
                .Add("TotalCreditCard", False, SqlDbType.Decimal)
                .Add("TotalDebitCard", False, SqlDbType.Decimal)
                .Add("TotalSRV", False, SqlDbType.Decimal)

                .Add("TotalAlipay", False, SqlDbType.Decimal)
                .Add("TotalComplimentary", False, SqlDbType.Decimal)
                .Add("TotalDiscountVoucher", False, SqlDbType.Decimal)
                .Add("TotalEPlus", False, SqlDbType.Decimal)
                .Add("TotalFoodPanda", False, SqlDbType.Decimal)
                .Add("TotalGCash", False, SqlDbType.Decimal)
                .Add("TotalGoldCard", False, SqlDbType.Decimal)
                .Add("TotalGrabFood", False, SqlDbType.Decimal)
                .Add("TotalLocalDelivery", False, SqlDbType.Decimal)
                .Add("TotalPayMaya", False, SqlDbType.Decimal)
                .Add("TotalShopeeEVoucher", False, SqlDbType.Decimal)
                .Add("TotalShopeePay", False, SqlDbType.Decimal)
                .Add("TotalSMAC", False, SqlDbType.Decimal)
                .Add("TotalVPlus", False, SqlDbType.Decimal)

                .Add("NoOfOneCents", False, SqlDbType.Int)
                .Add("NoOfFiveCents", False, SqlDbType.Int)
                .Add("NoOfTenCents", False, SqlDbType.Int)
                .Add("NoOfTwentyFiveCents", False, SqlDbType.Int)
                .Add("NoOfOnePeso", False, SqlDbType.Int)
                .Add("NoOfFivePesos", False, SqlDbType.Int)
                .Add("NoOfTenPesos", False, SqlDbType.Int)
                .Add("NoOfTwentyPesos", False, SqlDbType.Int)
                .Add("NoOfFiftyPesos", False, SqlDbType.Int)
                .Add("NoOfOneHundredPesos", False, SqlDbType.Int)
                .Add("NoOfTwoHundredPesos", False, SqlDbType.Int)
                .Add("NoOfFiveHundredPesos", False, SqlDbType.Int)
                .Add("NoOfOneThousandPesos", False, SqlDbType.Int)
                .Add("LastUpdated", False, SqlDbType.DateTime2)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")
            pDtOut.Columns.Add("LastUpdated")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "TransCashHandlingCashCount"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransCashHandlingCashCount")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportExpensesLogDetails(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView
            Dim datecovered As New List(Of String)()

            oError = ""

            DataImportLog.AddLog("Import from TransExpensesLogDetails")
            pCsvReader = New CsvReader(vInputZipFilename, "TransExpensesLogDetails")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("ExpenseType", True, SqlDbType.VarChar)
                .Add("ExpenseDate", False, SqlDbType.VarChar)
                .Add("FromLocation", False, SqlDbType.VarChar)
                .Add("ToLocation", False, SqlDbType.VarChar)
                .Add("Amount", True, SqlDbType.Decimal)
                .Add("AssociatedTo", False, SqlDbType.UniqueIdentifier)
                .Add("FowstID", True, SqlDbType.UniqueIdentifier)
                .Add("CostCenter", False, SqlDbType.UniqueIdentifier)
                .Add("Remarks", False, SqlDbType.VarChar)
                .Add("GPSLocation", False, SqlDbType.VarChar)
                .Add("DateCreated", True, SqlDbType.DateTime)
                .Add("LastUpdated", True, SqlDbType.DateTime)
                .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
                .Add("IsFromCheckInOut", False, SqlDbType.VarChar)
                .Add("IsDelete", False, SqlDbType.VarChar)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    AppendToLog(oError)
                    Continue Do
                End If
                pIU.TableName = "TransExpensesLogDetails"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    pIU("DateImported") = "getdate()"

                    pSql = "SELECT * FROM TransExpensesLogDetails WHERE ID = " & ReadyString(pLine("ID"))
                    Dim pRows As Object = mBusiness.ProData.Execute(pSql).Rows
                    Dim expense As New ExpensesController

                    If pRows.Count > 0 Then
                        Dim serverlastupdated As DateTime = CDate(pRows(0)("LastUpdated"))
                        Dim devicelastupdated As DateTime = CDate(pLine("LastUpdated"))
                        Dim datetimecompareresult = DateTime.Compare(devicelastupdated, serverlastupdated)

                        If pLine.ContainsKey("LastUpdatedBy") = False Then
                            pLine.Add("LastUpdatedBy", pLine("PeopleID"))
                        Else
                            If CStr2(pLine("LastUpdatedBy")) = "" Then
                                pLine("LastUpdatedBy") = pLine("PeopleID")
                            End If
                        End If

                        If pLine.ContainsKey("EncodedBy") = False Then
                            pLine.Add("EncodedBy", "")
                        End If

                        If pLine.ContainsKey("IsUnfinishedExpense") = False Then
                            pLine.Add("IsUnfinishedExpense", "")
                        End If

                        If pLine.ContainsKey("IsApproved") = False Then
                            pLine.Add("IsApproved", "")
                        End If

                        If pLine.ContainsKey("ApprovedOrRejectedBy") = False Then
                            pLine.Add("ApprovedOrRejectedBy", "")
                        End If

                        If pLine.ContainsKey("IsRejected") = False Then
                            pLine.Add("IsRejected", "")
                        End If

                        If pLine.ContainsKey("Particulars") = False Then
                            pLine.Add("Particulars", "")
                        End If

                        If pLine.ContainsKey("IsCreatedInWebApp") = False Then
                            pLine.Add("IsCreatedInWebApp", 0)
                        End If

                        If datetimecompareresult < 0 Then
                            'gi edit sa device pero na late ug flush, tapos giedit sa server before naflush ang data, maong isave nalang sa history table ang data from device
                            expense.SaveLogExpenseHistory(pLine, mBusiness.ProData)
                        ElseIf datetimecompareresult > 0 Then
                            'mas latest ang giedit sa device kesa sa server, iupdate ang server from device data then ibutang ang old data from server sa history table
                            pIU("LastUpdatedBy") = ReadyString(pLine("PeopleID"))
                            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                            If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                                pSql = pIU.GetInsert
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If
                            expense.SaveLogExpenseHistory(pRows(0), mBusiness.ProData)
                        Else
                            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))

                            If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                                pSql = pIU.GetInsert
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If
                        End If
                    Else
                        pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))

                        If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                            pSql = pIU.GetInsert
                            mBusiness.ProData.ExecuteSQL(pSql)
                        End If
                    End If

                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                    Dim expensedate As String

                    If pLine.ContainsKey("ExpenseDate") = False Then
                        expensedate = pLine("DateCreated")
                    Else
                        expensedate = pLine("ExpenseDate")
                    End If

                    pLine("ExpenseDate") = expensedate

                    expense.RecacheLogExpense(pLine, mBusiness.ProData)

                    If datecovered.Contains(Format(CDate(expensedate), "MM yyyy") & " " & pLine("PeopleID")) = False Then
                        datecovered.Add(Format(CDate(expensedate), "MM yyyy") & " " & pLine("PeopleID") & " " & pLine("FowstID"))
                    End If

                    'pSql = " SELECT ISNULL(SUM(Amount), 0) as TotalAmount from TransExpensesLogDetails " & vbCrLf &
                    '        " WHERE ExpenseType = " & ReadyString(pLine("ExpenseType")) & vbCrLf &
                    '        " AND PeopleID = " & ReadyString(pLine("PeopleID")) & vbCrLf &
                    '        " AND FowstID = " & ReadyString(pLine("FowstID")) & vbCrLf &
                    '        " AND convert(varchar(7), " & ReadyString(expensedate) & ", 126) = convert(varchar(7), ISNULL(ExpenseDate, DateCreated), 126) " & vbCrLf &
                    '        " AND ISNULL(IsDelete, 'False') = 'False' "

                    'Dim logtotal As Object = mBusiness.ProData.Execute(pSql).Rows
                    'Dim totalamount As Decimal = 0

                    'If logtotal.Count > 0 Then
                    '    totalamount = logtotal(0)("TotalAmount")
                    'Else
                    '    totalamount = pLine("Amount")
                    'End If

                    'pSql = "SELECT ID, Amount FROM TransLogExpensesCache " & vbCrLf &
                    '        " WHERE ExpenseType = " & ReadyString(pLine("ExpenseType")) & vbCrLf &
                    '        " AND PeopleID = " & ReadyString(pLine("PeopleID")) &
                    '        " AND FowstID = " & ReadyString(pLine("FowstID")) & vbCrLf &
                    '        " AND MonthCovered = " & ReadyString(Format(CDate(expensedate), "MM")) & vbCrLf &
                    '        " AND YearCovered = " & ReadyString(Format(CDate(expensedate), "yyyy"))

                    'Dim logexpensecache As Object = mBusiness.ProData.Execute(pSql).Rows

                    'Dim pTable As New InsertUpdate("TransLogExpensesCache")

                    'pTable("ExpenseType") = ReadyString(pLine("ExpenseType"))
                    'pTable("PeopleID") = ReadyString(pLine("PeopleID"))
                    'pTable("MonthCovered") = ReadyString(Format(CDate(expensedate), "MM"))
                    'pTable("YearCovered") = ReadyString(Format(CDate(expensedate), "yyyy"))
                    'pTable("FowstID") = ReadyString(pLine("FowstID"))
                    'pTable("Amount") = totalamount
                    'pTable("LastUpdated") = "getdate()"

                    'If logexpensecache.Count > 0 Then
                    '    pSql = pTable.GetUpdate(" ID = " & ReadyString(logexpensecache(0)("ID")))
                    'Else
                    '    pTable("ID") = ReadyString(Guid.NewGuid.ToString())
                    '    pSql = pTable.GetInsert()
                    'End If

                    'mBusiness.ProData.Execute(pSql)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            For i As Integer = 0 To datecovered.Count - 1
                ExpensesController.UpdateNetFundBalance(datecovered(i), mBusiness.ProData, datecovered(i).Split(" ")(3))
            Next

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransExpensesLogDetails")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportVouchers(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from LibraryVouchers")
            pCsvReader = New CsvReader(vInputZipFilename, "LibraryVouchers")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("ReferenceNo", True, SqlDbType.VarChar)
                .Add("ProductCode", False, SqlDbType.UniqueIdentifier)
                .Add("IsActive", False, SqlDbType.Bit)
                .Add("UploadedByID", False, SqlDbType.UniqueIdentifier)
                .Add("UploadedByName", False, SqlDbType.VarChar)
                .Add("FowstID", False, SqlDbType.UniqueIdentifier)
                .Add("CashierID", False, SqlDbType.UniqueIdentifier)
                .Add("CashierName", False, SqlDbType.VarChar)
                .Add("TransactionID", False, SqlDbType.UniqueIdentifier)
                .Add("TransactionDate", False, SqlDbType.DateTime)
                .Add("LastUpdated", False, SqlDbType.DateTime)
                .Add("DateImported", False, SqlDbType.DateTime)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")
            pDtOut.Columns.Add("LastUpdated")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "MobizLibraryVouchers"
                pIU("DateImported") = "getdate()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "LibraryVouchers")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportVDMLogs(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean
        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView
            oError = ""
            DataImportLog.AddLog("Import from TransVDMLogs")
            pCsvReader = New CsvReader(vInputZipFilename, "TransVDMLogs")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("FOWSTID", True, SqlDbType.UniqueIdentifier)
                .Add("DeviceID", True, SqlDbType.UniqueIdentifier)
                .Add("Page", True, SqlDbType.VarChar)
                .Add("Action", False, SqlDbType.VarChar)
                .Add("InvoiceID", False, SqlDbType.UniqueIdentifier)
                .Add("LogID", True, SqlDbType.UniqueIdentifier)
                .Add("UserID", False, SqlDbType.UniqueIdentifier)
                .Add("LastUpdated", True, SqlDbType.DateTime)
                .Add("DateImported", False, SqlDbType.DateTime)
            End With
            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")
            pDtOut.Columns.Add("LastUpdated")
            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "TransVDMLogs"
                pIU("DateImported") = "getdate()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)
                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop
            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransVDMLogs")
            End If
            If pDataImportLogAlreadyImported IsNot Nothing Then
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try
        Return True
    End Function
    Private Function ImportExpensesCheckInOutDetails(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from TransExpensesCheckInOutDetails")
            pCsvReader = New CsvReader(vInputZipFilename, "TransExpensesCheckInOutDetails")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("CheckIn", False, SqlDbType.DateTime)
                .Add("CheckOut", False, SqlDbType.DateTime)
                .Add("CheckInGPSLocation", False, SqlDbType.VarChar)
                .Add("CheckInLocationName", False, SqlDbType.VarChar)
                .Add("CheckInLogExpenseID", False, SqlDbType.UniqueIdentifier)
                .Add("CheckInCustomerOrgID", False, SqlDbType.UniqueIdentifier)
                .Add("CheckInCustomerOrgOutletID", False, SqlDbType.UniqueIdentifier)
                .Add("CheckOutGPSLocation", False, SqlDbType.VarChar)
                .Add("CheckOutLocationName", False, SqlDbType.VarChar)
                .Add("CheckOutLogExpenseID", False, SqlDbType.UniqueIdentifier)
                .Add("CheckOutCustomerOrgID", False, SqlDbType.UniqueIdentifier)
                .Add("CheckOutCustomerOrgOutletID", False, SqlDbType.UniqueIdentifier)
                .Add("Remarks", False, SqlDbType.VarChar)
                .Add("LastUpdated", True, SqlDbType.DateTime)
                .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")
            pDtOut.Columns.Add("LastUpdated")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    Continue Do
                End If
                pIU.TableName = "TransExpensesCheckInOutDetails"
                pIU("DateImported") = "getdate()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransExpensesCheckInOutDetails")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportExpensesReceiveFunds(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView
            Dim datecovered As New List(Of String)()

            oError = ""

            DataImportLog.AddLog("Import from TransExpensesReceiveFunds")
            pCsvReader = New CsvReader(vInputZipFilename, "TransExpensesReceiveFunds")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("ReceiveDate", True, SqlDbType.DateTime)
                .Add("ReceiveFrom", False, SqlDbType.UniqueIdentifier)
                .Add("OtherPerson", False, SqlDbType.VarChar)
                .Add("ReceiveBy", False, SqlDbType.UniqueIdentifier)
                .Add("FowstID", False, SqlDbType.UniqueIdentifier)
                .Add("Amount", True, SqlDbType.Decimal)
                .Add("Comment", False, SqlDbType.VarChar)
                .Add("DateCreated", False, SqlDbType.DateTime)
                .Add("LastUpdated", False, SqlDbType.DateTime)
                .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
                .Add("IsDelete", False, SqlDbType.VarChar)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")
            pDtOut.Columns.Add("LastUpdated")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    AppendToLog(oError)
                    Continue Do
                End If
                pIU.TableName = "TransExpensesReceiveFunds"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    pIU("DateImported") = "getdate()"

                    pSql = "SELECT * FROM TransExpensesReceiveFunds WHERE ID = " & ReadyString(pLine("ID").ToString())
                    Dim pRows As Object = mBusiness.ProData.Execute(pSql).Rows

                    If pRows.Count > 0 Then
                        Dim serverlastupdated As DateTime = CDate(pRows(0)("LastUpdated"))
                        Dim devicelastupdated As DateTime = CDate(pLine("LastUpdated"))
                        Dim datetimecompareresult = DateTime.Compare(devicelastupdated, serverlastupdated)
                        Dim expense As New ExpensesController

                        If pLine.ContainsKey("LastUpdatedBy") = False Then
                            pLine.Add("LastUpdatedBy", pLine("PeopleID").ToString())
                        Else
                            If CStr2(pLine("LastUpdatedBy")) = "" Then
                                pLine("LastUpdatedBy") = pLine("PeopleID").ToString()
                            End If
                        End If

                        If pLine.ContainsKey("EncodedBy") = False Then
                            pLine.Add("EncodedBy", "")
                        End If

                        If pLine.ContainsKey("OtherPerson") = False Then
                            pLine.Add("OtherPerson", "")
                        End If

                        If pLine.ContainsKey("ReceiveFrom") = False Then
                            pLine.Add("ReceiveFrom", "")
                        End If

                        If pLine.ContainsKey("FowstID") = False Then
                            pSql = "SELECT TOP 1 * FROM MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(vOrgID)
                            pLine.Add("FowstID", mBusiness.ProData.Execute(pSql).Rows(0)("ID").ToString())
                        End If

                        If datetimecompareresult < 0 Then
                            'gi edit sa device pero na late ug flush, tapos giedit sa server before naflush ang data, maong isave nalang sa history table ang data from device
                            expense.SaveReceiveFundHistory(pLine, mBusiness.ProData)
                        ElseIf datetimecompareresult > 0 Then
                            'mas latest ang giedit sa device kesa sa server, iupdate ang server from device data then ibutang ang old data from server sa history table
                            pIU("LastUpdatedBy") = ReadyString(pLine("PeopleID").ToString())
                            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID").ToString()))
                            If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                                pSql = pIU.GetInsert
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If
                            expense.SaveReceiveFundHistory(pRows(0), mBusiness.ProData)
                        Else
                            AppendToDebug("saving received fund 1")
                            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID").ToString()))

                            If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                                pSql = pIU.GetInsert
                                mBusiness.ProData.ExecuteSQL(pSql)
                            End If
                        End If
                    Else
                        pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID").ToString()))

                        If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                            pSql = pIU.GetInsert
                            mBusiness.ProData.ExecuteSQL(pSql)
                        End If
                    End If

                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)

                    If datecovered.Contains(Format(CDate(pLine("ReceiveDate")), "MM yyyy") & " " & pLine("PeopleID")) = False Then
                        datecovered.Add(Format(CDate(pLine("ReceiveDate")), "MM yyyy") & " " & pLine("PeopleID") & " " & pLine("FowstID"))
                    End If

                    pSql = " SELECT ISNULL(SUM(Amount), 0) as TotalAmount from TransExpensesReceiveFunds " & vbCrLf &
                            " WHERE PeopleID = " & ReadyString(pLine("PeopleID")) & vbCrLf &
                            " AND convert(varchar(7), " & ReadyString(pLine("ReceiveDate")) & ", 126) = convert(varchar(7), ReceiveDate, 126) " & vbCrLf &
                            " AND ISNULL(IsDelete, 'False') = 'False' AND FowstID =  " & ReadyString(pLine("FowstID"))

                    Dim logtotal As Object = mBusiness.ProData.Execute(pSql).Rows
                    Dim totalamount As Decimal = 0

                    If logtotal.Count > 0 Then
                        totalamount = logtotal(0)("TotalAmount")
                    Else
                        totalamount = pLine("Amount")
                    End If

                    pSql = "SELECT ID, Amount FROM TransReceiveFundsCache " & vbCrLf &
                            " WHERE PeopleID = " & ReadyString(pLine("PeopleID")) &
                            " AND MonthCovered = " & ReadyString(Format(CDate(pLine("ReceiveDate")), "MM")) & vbCrLf &
                            " AND YearCovered = " & ReadyString(Format(CDate(pLine("ReceiveDate")), "yyyy")) & vbCrLf &
                            " AND FowstID =  " & ReadyString(pLine("FowstID"))

                    Dim receivefundcache As Object = mBusiness.ProData.Execute(pSql).Rows

                    Dim pTable As New InsertUpdate("TransReceiveFundsCache")

                    pTable("FowstID") = ReadyString(pLine("FowstID"))
                    pTable("PeopleID") = ReadyString(pLine("PeopleID"))
                    pTable("MonthCovered") = ReadyString(Format(CDate(pLine("ReceiveDate")), "MM"))
                    pTable("YearCovered") = ReadyString(Format(CDate(pLine("ReceiveDate")), "yyyy"))
                    pTable("Amount") = totalamount
                    pTable("LastUpdated") = "getdate()"

                    If receivefundcache.Count > 0 Then
                        pSql = pTable.GetUpdate(" ID = " & ReadyString(receivefundcache(0)("ID")))
                    Else
                        pTable("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = pTable.GetInsert()
                    End If

                    mBusiness.ProData.Execute(pSql)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            For i As Integer = 0 To datecovered.Count - 1
                ExpensesController.UpdateNetFundBalance(datecovered(i), mBusiness.ProData, datecovered(i).Split(" ")(3))
            Next

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransExpensesReceiveFunds")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportExpensesNetFundBalance(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView
            Dim datecovered As New List(Of String)()

            oError = ""

            DataImportLog.AddLog("Import from TransNetFundBalance")
            pCsvReader = New CsvReader(vInputZipFilename, "TransNetFundBalance")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("NetFundBalance", False, SqlDbType.Decimal)
                .Add("MonthCovered", False, SqlDbType.VarChar)
                .Add("YearCovered", False, SqlDbType.VarChar)
                .Add("IsAdjustment", False, SqlDbType.Bit)
                .Add("OriginalAmount", False, SqlDbType.Decimal)
                .Add("AdjustedAmount", False, SqlDbType.Decimal)
                .Add("Status", False, SqlDbType.VarChar)
                .Add("LastUpdated", False, SqlDbType.DateTime)
                .Add("RequestedDate", False, SqlDbType.DateTime)
                .Add("ApprovedBy", False, SqlDbType.UniqueIdentifier)
                .Add("AdjustmentType", False, SqlDbType.VarChar)
                .Add("Remarks", False, SqlDbType.VarChar)
                .Add("FowstID", False, SqlDbType.VarChar)
                .Add("PeopleID", True, SqlDbType.UniqueIdentifier)
                .Add("IsDelete", False, SqlDbType.Bit)
                .Add("IsCancelRequest", False, SqlDbType.Bit)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")
            pDtOut.Columns.Add("LastUpdated")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    AppendToLog(oError)
                    Continue Do
                End If
                pIU.TableName = "TransNetFundBalance"
                pIU("DateImported") = "getdate()"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDrOut("LastUpdated") = pLine("LastUpdated")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransNetFundBalance")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportBeginningBalance(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Try
            Dim pCsvReader As CsvReader
            Dim pLine As Dictionary(Of String, String)
            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pBinder As DictionaryBinder
            Dim pDtOut, pDtOutItems As DataTable
            Dim pDrOut As DataRow
            Dim pNum As Integer
            Dim pDtItems As DataTable
            Dim pItemInitialized As Boolean
            Dim pDrInvoice As DataRow
            Dim pDataImportLogAlreadyImported As DataImportLog
            Dim pMaxSubNumber As Integer?
            Dim pDvItems As DataView

            oError = ""

            DataImportLog.AddLog("Import from TransDailyBeginningBalance")
            pCsvReader = New CsvReader(vInputZipFilename, "TransDailyBeginningBalance")
            pBinder = New DictionaryBinder
            With pBinder.Items
                .Add("ID", True, SqlDbType.UniqueIdentifier)
                .Add("BeginningBalanceDate", True, SqlDbType.VarChar)
                .Add("FOWSTID", True, SqlDbType.UniqueIdentifier)
                .Add("ProductID", True, SqlDbType.UniqueIdentifier)
                .Add("BegBalance", False, SqlDbType.Decimal)
                .Add("EndBalance", False, SqlDbType.Decimal)
                .Add("Sales", False, SqlDbType.Decimal)
                .Add("Receiving", False, SqlDbType.Decimal)
                .Add("SalesReturns", False, SqlDbType.Decimal)
                .Add("Transfers", False, SqlDbType.Decimal)
                .Add("InventoryCountVariance", False, SqlDbType.Decimal)
                .Add("PeopleID", False, SqlDbType.UniqueIdentifier)
            End With

            pDtOut = New DataTable
            pDtOut.Columns.Add("ID")

            Do
                pLine = pCsvReader.ReadLine
                If pLine Is Nothing Then Exit Do
                pBinder.Source = pLine
                pIU = pBinder.CreateInsertUpdate(oError)
                If pIU Is Nothing Then
                    AppendToLog(oError)
                    Continue Do
                End If
                pIU.TableName = "TransDailyBeginningBalance"
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                Try
                    If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                        pSql = pIU.GetInsert
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                    pDrOut = pDtOut.NewRow
                    pDrOut("ID") = pLine("ID")
                    pDtOut.Rows.Add(pDrOut)

                Catch ex As Exception
                    oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                    AppendToLog(oError, , mLogFile)
                    oError = JsonDictionary.Serialize("Error", oError)
                    Return False
                End Try
            Loop

            If mConsolidationMode = False Then
                rZipOut.AddTableCsv(pDtOut, "TransDailyBeginningBalance")
            End If

            If pDataImportLogAlreadyImported IsNot Nothing Then
                ' ReSharper disable VBWarnings::BC42104
                pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
                ' ReSharper restore VBWarnings::BC42104
            End If
        Catch ex As System.Exception
            Try
                AppendToDebug("vbM4BQ4Y8KBB1 " & GetStackTrace(, ex))
            Catch ex2 As Exception
                AppendToDebug("vbL9CUN3GV6U! " + ex2.Message)
            End Try
        End Try

        Return True

    End Function

    Private Function ImportBomMaker(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
    ByVal vOrgID As String, ByVal vUuid As String,
    ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBinder As DictionaryBinder
        Dim pDtOut, pDtOutItems As DataTable
        Dim pDrOut As DataRow
        Dim pNum As Integer
        Dim pDtItems As DataTable
        Dim pDtContents As DataTable
        Dim pItemInitialized As Boolean
        Dim pDataImportLogAlreadyImported As DataImportLog
        Dim pMaxSubNumber As Integer?
        Dim pDvItems As DataView
        Dim pDvContents As DataView

        oError = ""

        DataImportLog.AddLog("Import from TransBomMaker")
        pCsvReader = New CsvReader(vInputZipFilename, "TransBomMaker")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("UserID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", True, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", True, SqlDbType.UniqueIdentifier)

            .Add("AssignedToID", False, SqlDbType.UniqueIdentifier)
            .Add("DateNeeded", False, SqlDbType.DateTime)
            .Add("Num", True, SqlDbType.Int)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            mBusiness.ProData.RollbackTransaction(True, True)
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return False
            End If
            If pItemInitialized = False Then
                pItemInitialized = True
                pDtItems = CreateBomMakerProductsDataTable(vInputZipFilename, oError)
                If CStr2(oError) <> "" Then Return False

                pDtContents = CreateBomMakerContentsDataTable(vInputZipFilename, oError)
                If CStr2(oError) <> "" Then Return False
            End If
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If

            pIU.TableName = "TransBomMaker"
            mBusiness.ProData.BeginTransaction()
            Try
                pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                pSql = "DELETE FROM TransBomMakerContents WHERE ParentID IN (" &
                    " SELECT ID" &
                    " FROM TransBomMakerProducts" &
                    " WHERE ParentID = " & ReadyString(pLine("ID")) & ")"
                pSql = mBusiness.ProData.ExecuteSQL(pSql) ' get the number of effected records for debugging

                pSql = "DELETE FROM TransBomMakerProducts WHERE ParentID = " & ReadyString(pLine("ID"))
                pSql = mBusiness.ProData.ExecuteSQL(pSql) ' get the number of effected records for debugging

                ' BEGIN insert items
                pDvItems = New DataView(pDtItems, "ParentID = " & ReadyString(pLine("ID")), "", DataViewRowState.CurrentRows)
                For Each pDrvItem As DataRowView In pDvItems
                    If CDec2(pDrvItem("Quantity")) = 0 Then
                        Continue For
                    End If
                    pIU = New InsertUpdate("TransBomMakerProducts")
                    pIU("ID") = ReadyString(pDrvItem("ID"))
                    pIU("ParentID") = ReadyString(pLine("ID"))
                    pIU("ProductID") = ReadyString(pDrvItem("ProductID"))
                    pIU("Quantity") = CDec2(pDrvItem("Quantity"))
                    pIU("UnitMeasureID") = ReadyString(pDrvItem("UnitMeasureID"))
                    pSql = pIU.GetInsert()
                    mBusiness.ProData.ExecuteSQL(pSql)

                    ' BEGIN insert contents
                    pDvContents = New DataView(pDtContents, "ParentID = " & ReadyString(pDrvItem("ID")), "", DataViewRowState.CurrentRows)
                    For Each pDrvContent As DataRowView In pDvContents
                        If CDec2(pDrvContent("Quantity")) = 0 Then
                            Continue For
                        End If
                        pIU = New InsertUpdate("TransBomMakerContents")
                        pIU("ID") = ReadyString(pDrvContent("ID"))
                        pIU("ParentID") = ReadyString(pDrvContent("ParentID"))
                        pIU("ProductID") = ReadyString(pDrvContent("ProductID"))
                        pIU("Quantity") = CDec2(pDrvContent("Quantity"))
                        If pDrvContent("UnitMeasureID") IsNot DBNull.Value Then
                            pIU("UnitMeasureID") = ReadyString(pDrvContent("UnitMeasureID"))
                        End If
                        If pDrvContent("UnitMeasure") IsNot DBNull.Value Then
                            pIU("UnitMeasure") = ReadyString(pDrvContent("UnitMeasure"))
                        End If
                        If pDrvContent("BaseUnitMeasureID") IsNot DBNull.Value Then
                            pIU("BaseUnitMeasureID") = ReadyString(pDrvContent("BaseUnitMeasureID"))
                        End If
                        If pDrvContent("QtyInBaseUnit") IsNot DBNull.Value Then
                            pIU("QtyInBaseUnit") = CDec2(pDrvContent("QtyInBaseUnit"))
                        End If
                        pSql = pIU.GetInsert()
                        mBusiness.ProData.ExecuteSQL(pSql)
                    Next pDrvContent
                    ' END insert contents
                Next pDrvItem
                ' END insert items
                mBusiness.TransBomMaker.AddToInventoryLedger(pLine("ID"), OrgID)
                mBusiness.ProData.CommitTransaction()

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction()
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
            pDrOut = pDtOut.NewRow
            pDrOut("ID") = pLine("ID")
            pDtOut.Rows.Add(pDrOut)
        Loop

        If pDtOut.Rows.Count = 0 Then
            If CStr2(oError).Trim <> "" Then
                Return False
            End If
        End If

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "TransBomMaker")
        End If

        If pDataImportLogAlreadyImported IsNot Nothing Then
            ' ReSharper disable VBWarnings::BC42104
            pDataImportLogAlreadyImported.Save(DataImportLog.eResultType.AlreadyImported)
            ' ReSharper restore VBWarnings::BC42104
        End If

        Return True

    End Function
    'additional by janine feb 3 2015
    Private Function ImportLibraryDevicePrinterFowst(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
       ByVal vOrgID As String, ByVal vUuid As String,
       ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String
        DataImportLog.AddLog("Import from LibraryDevicePrinterFowst")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryDevicePrinterFowst")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FowstID", False, SqlDbType.VarChar)
            .Add("PrinterID", False, SqlDbType.VarChar)
            .Add("PrinterIPAddress", False, SqlDbType.VarChar)
            .Add("PrinterName", False, SqlDbType.VarChar)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryDevicePrinterFowst"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)


                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevicePrinter'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryDevicePrinterFowst")
        End If

        Return True

    End Function

    Private Function ImportLibraryDeviceFowstPrinter(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
   ByVal vOrgID As String, ByVal vUuid As String,
   ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String
        DataImportLog.AddLog("Import from LibraryDeviceFowstPrinter")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryDeviceFowstPrinter")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", False, SqlDbType.UniqueIdentifier)
            .Add("IsActive", False, SqlDbType.Bit)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("PrinterFowstID", False, SqlDbType.UniqueIdentifier)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "LibraryDeviceFowstPrinter"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryDeviceFowstPrinter")
        End If

        Return True

    End Function

    Private Function ImportLibraryDeviceFowst(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String
        DataImportLog.AddLog("Import from LibraryDeviceFowst")
        pCsvReader = New CsvReader(vInputZipFilename, "LibraryDeviceFowst")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ZReadCount", False, SqlDbType.Int)
            .Add("VoidNumber", False, SqlDbType.Int)
            .Add("DeviceFowstTransactionNumber", False, SqlDbType.Int)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            If pLine.ContainsKey("DeviceFowstTransactionNumber") Then
                pIU("DeviceFowstTransactionNumber") = "CASE WHEN ISNULL(DeviceFowstTransactionNumber, 0) < " & CInt2(pLine("DeviceFowstTransactionNumber")) & " THEN " & CInt2(pLine("DeviceFowstTransactionNumber")) & " ELSE DeviceFowstTransactionNumber END "
            End If
            pIU.TableName = "LibraryDeviceFowst"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

                pSql2 = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDeviceFowst'"
                mBusiness.ProData.ExecuteSQL(pSql2)

            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "LibraryDeviceFowst")
        End If

        Return True

    End Function

    Private Function ImportActionLogs(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pSql2 As String

        DataImportLog.AddLog("Import from TransActionLogs")
        pCsvReader = New CsvReader(vInputZipFilename, "TransActionLogs")
        pBinder = New DictionaryBinder

        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ActionDate", False, SqlDbType.VarChar)
            .Add("EmployeeName", False, SqlDbType.VarChar)
            .Add("FOWSTID", False, SqlDbType.VarChar)
            .Add("DeviceID", False, SqlDbType.VarChar)
            .Add("Action", False, SqlDbType.VarChar)
            .Add("OriginalValue", False, SqlDbType.VarChar)
            .Add("NewValue", False, SqlDbType.VarChar)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransActionLogs"
            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))
            Try
                If mBusiness.ProData.ExecuteSQL(pSql, True) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "TransActionLogs")
        End If

        Return True

    End Function
    ' end of additional 

    Private Sub HandleDeviceFowstTransactionNumber(ByRef rIU As InsertUpdate, vLine As Dictionary(Of String, String), ByRef oDeviceFowstTransactionNumber As Integer)

        Dim pDrDeviceFowst = mBusiness.LibraryDeviceFowst.ByDeviceFowst(vLine("DeviceID"), vLine("FowstID"))
        Dim pLastNumberInLibrary As Integer
        If pDrDeviceFowst IsNot Nothing Then
            pLastNumberInLibrary = CInt2(pDrDeviceFowst("DeviceFowstTransactionNumber"))
        End If

        If mMobizAppVersion = "2" AndAlso vLine.ContainsKey("DeviceFowstTransactionNumber") Then
            ' rIU("DeviceFowstTransactionNumber") ' this is already set
            oDeviceFowstTransactionNumber = CInt2(vLine("DeviceFowstTransactionNumber"))
        Else
            oDeviceFowstTransactionNumber = pLastNumberInLibrary + 1
            rIU("DeviceFowstTransactionNumber") = oDeviceFowstTransactionNumber
        End If

        If pLastNumberInLibrary < oDeviceFowstTransactionNumber Then
            mBusiness.LibraryDeviceFowst.UpdateLastNumber("DeviceFowstTransactionNumber", vLine("FowstID"), vLine("DeviceID"), oDeviceFowstTransactionNumber, Nothing)
        End If

    End Sub

    Private Function ImportMemo(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String

        DataImportLog.AddLog("Import from TransMemo")
        pCsvReader = New CsvReader(vInputZipFilename, "TransMemo")
        pBinder = New DictionaryBinder

        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("UserID", True, SqlDbType.VarChar)
            .Add("FowstID", True, SqlDbType.VarChar)
            .Add("DeviceID", False, SqlDbType.VarChar)
            .Add("Reason", True, SqlDbType.VarChar)
            .Add("CheckOutMethod", False, SqlDbType.VarChar)
            .Add("MemoDate", True, SqlDbType.Date)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("ApprovedBy", False, SqlDbType.UniqueIdentifier)
            .Add("DeviceFowstTransactionNumber", False, SqlDbType.Int)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            If pLine.ContainsKey("FowstID") = False _
                OrElse String.IsNullOrWhiteSpace(pLine("FowstID")) _
                OrElse CStr2(pLine("FowstID")).ToLower() = "null" Then

                If pLine.ContainsKey("DeviceID") = False _
                    OrElse String.IsNullOrWhiteSpace(pLine("DeviceID")) Then

                    Dim pDrDevice = mBusiness.LibraryDevice.ByUuid(Uuid)
                    If pDrDevice Is Nothing Then
                        AppendToLog("vbG2KRDDL6ER!: ImportMemo oError = DeviceID and FowstID are both null")
                        Continue Do
                    Else
                        pLine("DeviceID") = CStr2(pDrDevice("ID"))
                    End If
                End If

                Dim pDtDeviceFowsts = mBusiness.LibraryDeviceFowst.ByDeviceID(pLine("DeviceID"))
                If pDtDeviceFowsts.Rows.Count = 1 Then
                    pLine("FowstID") = CStr2(pDtDeviceFowsts.Rows(0)("FowstID"))
                Else
                    pLine("FowstID") = ReadyGuid(Guid.Empty)
                    AppendToLog("vbW2DH78EJMT!: ImportMemo. LibraryDeviceFowst.ByDeviceID must return 1 record. Actual record count: " & pDtDeviceFowsts.Rows.Count & "; MemoID=" & pLine("ID") & "; DeviceID=" & pLine("DeviceID"))
                End If
            End If
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                AppendToLog("ref: vbCASTQREU6V!" & vbCrLf & oError, , mLogFile)
                Continue Do
            End If
            pIU.TableName = "TransMemo"
            Try
                mBusiness.ProData.BeginTransaction()
                HandleDeviceFowstTransactionNumber(rIU:=pIU, vLine:=pLine, oDeviceFowstTransactionNumber:=1234567) ' oDeviceFowstTransactionNumber param does not matter here
                If pLine.ContainsKey("DeviceID") = False Then
                    pLine.Add("DeviceID", DeviceID)
                End If
                pSql = "SELECT COUNT(*) FROM " & pIU.TableName & vbCrLf &
                    " WHERE ID = " & ReadyString(pLine("ID"))

                If Now.Date = New Date(2021, 4, 27) Then
                    AppendToLog("ref: vbPASKJGJ7EH!" & vbCrLf & pSql, , mLogFile)
                End If

                If CInt2(mBusiness.ProData.ExecuteScalar(pSql)) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                mBusiness.ProData.CommitTransaction()
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)

                If Now.Date = New Date(2021, 4, 27) Then
                    AppendToLog("ref: vbDXVYLZJH4A!", , mLogFile)
                End If
            Catch ex As Exception
                mBusiness.ProData.RollbackTransaction(, True)
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "TransMemo")
        End If

        Return True

    End Function

    Private Function ImportMemoSalesReturn(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
        ByVal vOrgID As String, ByVal vUuid As String,
        ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pMemoIDs As New List(Of String)

        DataImportLog.AddLog("Import from TransMemoSalesReturn")
        pCsvReader = New CsvReader(vInputZipFilename, "TransMemoSalesReturn")
        pBinder = New DictionaryBinder
        oError = Nothing

        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("MemoID", True, SqlDbType.VarChar)
            .Add("CustomerID", True, SqlDbType.VarChar)
            .Add("Reason", True, SqlDbType.VarChar)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("InvoiceID", True, SqlDbType.VarChar)
            .Add("ProductID", True, SqlDbType.VarChar)
            .Add("UnitMeasure", True, SqlDbType.VarChar)
            .Add("QuantityReturned", True, SqlDbType.Decimal)
            .Add("ActualQuantity", True, SqlDbType.Decimal)
            .Add("UnitPrice", True, SqlDbType.Decimal)
            .Add("TotalAmount", True, SqlDbType.Decimal)
            .Add("TaxAmount", False, SqlDbType.Decimal)
            .Add("UnitDiscountAmount", False, SqlDbType.Decimal)
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        If Now.Date = New Date(2021, 1, 14) Then
            AppendToLog("vb7WSXRMF39S!: ImportMemoSalesReturn. before loop")
        End If

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                Continue Do
            End If
            pIU.TableName = "TransMemoSalesReturn"
            Try
                If Now.Date = New Date(2021, 1, 14) Then
                    AppendToLog("VB2A4TAKW89R!: ImportMemoSalesReturn. " & pLine("ID"))
                End If

                pSql = "SELECT COUNT(*) FROM " & pIU.TableName & vbCrLf &
                    " WHERE ID = " & ReadyString(pLine("ID"))

                If CInt2(mBusiness.ProData.ExecuteScalar(pSql)) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If
                mBusiness.TransMemoSalesReturn.FillMemosFowstID(pLine("MemoID"))

                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
                If pMemoIDs.Contains(pLine("MemoID")) = False Then
                    pMemoIDs.Add(pLine("MemoID"))
                End If
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        For Each pMemoID As String In pMemoIDs
            pSql = "SELECT FowstID " & vbCrLf &
            " FROM TransMemo" & vbCrLf &
            " WHERE ID = " & ReadyString(pMemoID)
            Dim pFowstID = CStr2(mBusiness.ProData.ExecuteScalar(pSql))
            If String.IsNullOrWhiteSpace(pFowstID) Then
                pFowstID = Guid.NewGuid.ToString() ' ???
            End If
            mBusiness.TransMemoSalesReturn.AddToInventoryLedger(pMemoID, pFowstID, , OrgID)
        Next

        If pDtOut.Rows.Count = 0 Then
            If String.IsNullOrWhiteSpace(oError) = False Then
                Throw New ApplicationException(oError & ". ref: vbPCSGCTQXUD!")
                Return False
            End If
        End If

        If Now.Date = New Date(2021, 1, 14) Then
            AppendToLog("VBSFYVYQR58S!: ImportMemoSalesReturn: " & pDtOut.Rows.Count)
        End If
        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "TransMemoSalesReturn")
        End If

        Return True

    End Function

    Private Function ImportSalesReturnLedger(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
    ByVal vOrgID As String, ByVal vUuid As String,
    ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOtherPayments() As String
        DataImportLog.AddLog("Import from TransSalesReturnLedger")
        pCsvReader = New CsvReader(vInputZipFilename, "TransSalesReturnLedger")
        pBinder = New DictionaryBinder

        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("SourceMemoID", True, SqlDbType.UniqueIdentifier)
            .Add("SourceInvoiceID", False, SqlDbType.UniqueIdentifier)
            .Add("DestinationPaymentID", False, SqlDbType.UniqueIdentifier)
            .Add("Amount", True, SqlDbType.Decimal)
            .Add("UserID", True, SqlDbType.VarChar)
            .Add("LastUpdated", True, SqlDbType.DateTime)
            .Add("FowstID", True, SqlDbType.VarChar)
            .Add("DeviceID", False, SqlDbType.VarChar)
            .Add("SrvNum", False, SqlDbType.Decimal)
            .Add("RefundNum", False, SqlDbType.Decimal)
            .Add("Pin", False, SqlDbType.VarChar)
            .Add("GrossRefundAmount", False, SqlDbType.Decimal)
            .Add("GrossSrvAmount", False, SqlDbType.Decimal)
            .Add("DeviceFowstTransactionNumber", False, SqlDbType.Int)
            Dim pFileText = FileText(Common.ServerPath & "Content/OtherPaymentTypes.txt")
            Dim pSplits = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            For Each pSplit In pSplits
                .Add("GrossRefundAmount" & pSplit, False, SqlDbType.Decimal)
            Next
        End With
        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")

        If Now.Date = New Date(2021, 2, 2) Then
            AppendToLog("vb2JYNYU8LGX!: ImportSalesReturnLedger.")
        End If

        If Now.Date = New Date(2021, 5, 14) Then
            AppendToLog("vbYUMRGQGMDB!: ImportSalesReturnLedger.")
        End If

        Do
            Dim pInsertMode As Boolean = False
            pLine = pCsvReader.ReadLine

            If pLine Is Nothing Then Exit Do

            If Now.Date = New Date(2021, 5, 14) Then
                AppendToLog("vbYCNA66UYVU!: ImportSalesReturnLedger.")
            End If

            If pLine.ContainsKey("DeviceID") = False Then
                pLine.Add("DeviceID", DeviceID)
            ElseIf String.IsNullOrWhiteSpace(pLine("DeviceID")) Then
                pLine("DeviceID") = DeviceID
            End If

            If pLine.ContainsKey("FowstID") = False _
                OrElse String.IsNullOrWhiteSpace(pLine("FowstID")) _
                OrElse CStr2(pLine("FowstID")).ToLower() = "null" Then

                If pLine.ContainsKey("DeviceID") = False _
                    OrElse String.IsNullOrWhiteSpace(pLine("DeviceID")) Then

                    AppendToLog("vbCJX7WT8K74!: ImportSalesReturnLedger oError = DeviceID and FowstID are both null")
                    Continue Do
                End If

                Dim pDtDeviceFowsts = mBusiness.LibraryDeviceFowst.ByDeviceID(pLine("DeviceID"))
                If pDtDeviceFowsts.Rows.Count = 1 Then
                    pLine("FowstID") = CStr2(pDtDeviceFowsts.Rows(0)("FowstID"))
                Else
                    pLine("FowstID") = ReadyGuid(Guid.Empty)
                    AppendToLog("vb68K9Y57BKJ!: ImportSalesReturnLedger. LibraryDeviceFowst.ByDeviceID must return 1 record. Actual record count: " & pDtDeviceFowsts.Rows.Count)
                End If
            End If

            If pLine.ContainsKey("SourceInvoiceID") = False _
                OrElse String.IsNullOrWhiteSpace(pLine("SourceInvoiceID")) Then

                If pLine.ContainsKey("SourceMemoID") = False _
                    OrElse String.IsNullOrWhiteSpace(pLine("SourceMemoID")) Then

                    oError = "vbJ3X3S88DEN!: ImportSalesReturnLedger oError = SourceInvoiceID and SourceMemoID are both null"
                    AppendToLog(oError)
                    Continue Do
                End If

                Dim pDrMemoSalesReturn = mBusiness.TransMemoSalesReturn.GetMemoSalesReturn(pLine("SourceMemoID"))
                If pDrMemoSalesReturn IsNot Nothing Then
                    pLine("SourceInvoiceID") = CStr2(pDrMemoSalesReturn("InvoiceID"))
                    AppendToLog("vbAFSNPTWY5Y! ImportSalesReturnLedger: InvoiceID has been resolved for MemoID" & pLine("SourceMemoID"))
                Else
                    oError = "vbHVAWRFE6WH!: ImportSalesReturnLedger oError = TransMemoSalesReturn record with MemoID" & pLine("SourceMemoID") & " was not found"
                    AppendToLog(oError)
                End If
            End If

            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                AppendToLog("vbAATKHSCUEG!: ImportSalesReturnLedger oError = " & oError)
                Return False
            End If

            If Now.Date = New Date(2021, 5, 14) Then
                AppendToLog("vbWLRSN3BWRQ!: ImportSalesReturnLedger.")
            End If

            pIU.TableName = "TransSalesReturnLedger"
            Try
                pSql = "SELECT COUNT(*) FROM " & pIU.TableName & vbCrLf &
                    " WHERE ID = " & ReadyString(pLine("ID"))

                If CInt2(mBusiness.ProData.ExecuteScalar(pSql)) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                    pInsertMode = True
                    Dim pFileText2 = FileText(Common.ServerPath & "Content/OtherPaymentTypes.txt")
                    Dim pSplits2 = pFileText2.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                    Dim HasOtherPaymentType As Boolean = False

                    AppendToDebug(pSplits2.Length)

                    For Each pSplit In pSplits2
                        If pLine.ContainsKey("GrossRefundAmount" & pSplit) Then
                            HasOtherPaymentType = True

                            pSql = "Update CacheInvoices set " & EscapeSqlName(pSplit) & "=" & EscapeSqlName(pSplit) & "-" & CDec2(EscapeSqlName(pLine("GrossRefundAmount" & pSplit))) & vbCrLf &
                                    "WHERE ID = " & ReadyString(pLine("SourceInvoiceID")) & vbCrLf

                            AppendToDebug(pSql)

                            mBusiness.ProData.ExecuteSQL(pSql)

                            pSql = "Update TransZReadCache set OtherTypeAmount = OtherTypeAmount - " & CDec2(EscapeSqlName(pLine("GrossRefundAmount" & pSplit))) & vbCrLf &
                                    "where CONVERT(date,SalesDate) = " & ReadyString(pLine("LastUpdated")) & vbCrLf &
                                    " AND  DEVICEID = " & ReadyString(DeviceID) & vbCrLf &
                                    " AND  FOWSTID = " & ReadyString(pLine("FowstID"))

                            mBusiness.ProData.ExecuteSQL(pSql)
                            AppendToDebug(pSql)

                        End If
                    Next
                    If Not HasOtherPaymentType Then
                        pSql = "Update CacheInvoices set CashNetOfChange = CashNetOfChange -" & CDec2(pLine("Amount")) & vbCrLf &
                                    " WHERE ID = " & ReadyString(pLine("SourceInvoiceID")) & vbCrLf
                        AppendToDebug(pSql)
                        mBusiness.ProData.ExecuteSQL(pSql)
                    End If
                End If

                Dim pSeriesFieldName As String ' POSLastSRVNo or POSLastRefNo
                Dim pUpdateFieldName As String  ' RefundNum or SrvNum
                Dim pFunctionName As String ' GetOrganizationFOWSTRefNo or GetOrganizationFOWSTSRVNo

                If pLine.ContainsKey("RefundNum") _
                    AndAlso CDec2(pLine("RefundNum")) > 0 Then

                    pSeriesFieldName = "POSLastRefNo"
                    pUpdateFieldName = "RefundNum"
                    pFunctionName = "GetOrganizationFOWSTRefNo"
                Else
                    pSeriesFieldName = "POSLastSRVNo"
                    pUpdateFieldName = "SrvNum"
                    pFunctionName = "GetOrganizationFOWSTSRVNo"
                End If

                'get top 1 fowst from librarydevicefowst
                'AppendToDebug("ABCDEFG")
                'AppendToDebug(CStr2(pLine("FowstID")))
                'AppendToDebug(CStr2(pLine("DeviceID")))
                'AppendToDebug(CStr2(pLine("ID")))

                If CStr2(pLine("FowstID")) = vbNullString OrElse CStr2(pLine("FowstID")) = "null" Then
                    pSql = "Select top 1 FowstID from librarydevicefowst WHERE DeviceID = " & ReadyString(pLine("DeviceID"))

                    pLine("FowstID") = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

                    AppendToDebug(CStr2(pLine("FowstID")))

                End If

                pIU = New InsertUpdate("LibraryDeviceFowst")
                pIU(pSeriesFieldName) = "(SELECT MAX(Num)" & vbCrLf &
                    " FROM (VALUES(" & CInt2(pLine(pUpdateFieldName)) & "), (dbo." & pFunctionName & "(" & ReadyString(pLine("FowstID")) & ", " & ReadyString(pLine("DeviceID")) & "))) AS AllNum(Num))"
                pSql = pIU.GetUpdate("FOWSTID = " & ReadyString(pLine("FowstID"))) &
                    " AND DeviceID = " & ReadyString(pLine("DeviceID"))
                If mBusiness.ProData.ExecuteSQL(pSql) = 0 Then
                    pIU("FOWSTID") = ReadyString(pLine("FowstID"))
                    pIU("DeviceID") = ReadyString(pLine("DeviceID"))
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                If pInsertMode Then
                    mBusiness.TransMemoSalesReturn.FillSalesReturnLedgersFowstID(pLine("ID"))
                    mBusiness.TransSalesReturnLedger.UpdateZReadCache(CStr2(pLine("DeviceID")), CStr2(pLine("FowstID")), CStr2(pLine("SourceMemoID")))
                    mBusiness.TransSalesReturnLedger.UpdateEJournalCache(CStr2(pLine("SourceMemoID")), CStr2(pLine("FowstID")), CStr2(pLine("DeviceID")))
                End If

                AppendToLog("vbMGVF8P7J37!: ImportSalesReturnLedger. " & pLine("ID"))
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDtOut.Rows.Add(pDrOut)
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        If mConsolidationMode = False Then
            rZipOut.AddTableCsv(pDtOut, "TransSalesReturnLedger")
        End If

        Return True

    End Function

    Private Function ImportTenantMonitoringLogs(ByVal vInputZipFilename As String, ByRef rJsonOut As JsonDictionary, ByRef rZipOut As ProZip,
       ByVal vOrgID As String, ByVal vUuid As String,
       ByRef oError As String) As Boolean

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDrOut As DataRow
        Dim pLine As Dictionary(Of String, String)
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pOtherPayments() As String
        DataImportLog.AddLog("Import from TenantMonitoringLogs")
        pCsvReader = New CsvReader(vInputZipFilename, "TenantMonitoringLogs")
        pBinder = New DictionaryBinder

        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("FOWSTID", True, SqlDbType.UniqueIdentifier)
            .Add("SalesDate", True, SqlDbType.VarChar)
            .Add("LastTransactionID", False, SqlDbType.UniqueIdentifier)
            .Add("Mall", False, SqlDbType.VarChar)
            .Add("FileStatus", False, SqlDbType.VarChar)
            .Add("Reported", False, SqlDbType.VarChar)
            .Add("ReportDate", False, SqlDbType.DateTime)
            .Add("LastUpdated", False, SqlDbType.DateTime)
            .Add("UserID", False, SqlDbType.UniqueIdentifier)
            .Add("DeviceID", False, SqlDbType.UniqueIdentifier)
            .Add("EndOfDayCounter", False, SqlDbType.Int)
            .Add("FirstPrintedDate", False, SqlDbType.DateTime)
            .Add("FirstPrintedBy", False, SqlDbType.UniqueIdentifier)
            .Add("NextPrintedDate", False, SqlDbType.DateTime)
            .Add("NextPrintedBy", False, SqlDbType.UniqueIdentifier)
        End With

        pDtOut = New DataTable
        pDtOut.Columns.Add("ID")
        pDtOut.Columns.Add("LastUpdated")

        If Now.Date = New Date(2021, 2, 2) Then
            AppendToLog("vb2JYNYU8LGX!: TenantMonitoringLogs.")
        End If

        If Now.Date = New Date(2021, 5, 14) Then
            AppendToLog("vbYUMRGQGMDB!: TenantMonitoringLogs.")
        End If

        Do
            pLine = pCsvReader.ReadLine

            If pLine Is Nothing Then Exit Do

            pBinder.Source = pLine
            pIU = pBinder.CreateInsertUpdate(oError)
            If pIU Is Nothing Then
                AppendToLog("vbAATKHSCUEG!: TenantMonitoringLogs oError = " & oError)
                Return False
            End If

            pIU.TableName = "TenantMonitoringLogs"

            If mBusiness.ProData.Execute("SELECT ID FROM TenantMonitoringLogs WHERE ID = " & ReadyString(pLine("ID")) & " AND FileStatus IS NULL").Rows.Count > 0 Then
                pIU("Reported") = 0
                pIU("ReportDate") = "NULL"
            End If

            pIU("DateImported") = "getdate()"
            pIU("LastUpdated") = "getdate()"

            Dim lastid As String
            If pLine.ContainsKey("LastTransactionID") Then
                lastid = CStr2(mBusiness.ProData.ExecuteScalar("SELECT TOP 1 ID FROM Transinvoice WHERE InvoiceNumber <> 0 AND FOWSTID = " & ReadyString(pLine("FOWSTID")) & " AND DeviceID = " & ReadyString(pLine("DeviceID")) & " AND CONVERT(DATE, Invoicedate) = " & ReadyString(Format(CDate(pLine("SalesDate")), "MM/dd/yyyy")) & " ORDER BY InvoiceDate DESC"))
                If String.IsNullOrWhiteSpace(lastid) Then
                    lastid = "NULL"
                Else
                    lastid = ReadyString(lastid)
                End If
            Else
                lastid = "NULL"
            End If

            pIU("LastTransactionID") = lastid

            Dim pEndOfDayCounter As String
            pLine.TryGetValue("EndOfDayCounter", pEndOfDayCounter)

            If pLine("Mall") = "Robinson" _
                AndAlso CStr2(pEndOfDayCounter) = "" _
                AndAlso CInt2(mBusiness.ProData.ExecuteScalar("SELECT ISNULL(EndOfDayCounter, 0) FROM TenantMonitoringLogs WHERE ID = " & ReadyString(pLine("ID")))) = 0 Then

                pSql = "SELECT ISNULL(MAX(EndOfDayCounter), 0) + 1 FROM TenantMonitoringLogs WHERE FOWSTID = " & ReadyString(pLine("FOWSTID")) & " AND DeviceID = " & ReadyString(pLine("DeviceID"))
                pIU("EndOfDayCounter") = CInt2(mBusiness.ProData.ExecuteScalar(pSql))
            End If

            pSql = pIU.GetUpdate("ID = " & ReadyString(pLine("ID")))

            Try

                'pSql = "SELECT COUNT(*) FROM " & pIU.TableName & vbCrLf &
                '    " WHERE ID = " & ReadyString(pLine("ID"))

                If mBusiness.ProData.ExecuteSQL(pSql) = 0 Then
                    pSql = pIU.GetInsert
                    mBusiness.ProData.ExecuteSQL(pSql)
                End If

                AppendToLog("vbMGVF8P7J37!: TenantMonitoringLogs. " & pLine("ID"))
                pDrOut = pDtOut.NewRow
                pDrOut("ID") = pLine("ID")
                pDrOut("LastUpdated") = pLine("LastUpdated")
                pDtOut.Rows.Add(pDrOut)

                pSql = "SELECT ID, FOWSTMallName FROM MobizLibraryOrganizationFOWST WHERE ID = " & ReadyString(pLine("FOWSTID".ToString())) & vbCrLf &
                        " AND (FOWSTMallName = 'Megaworld' OR FOWSTMallName = 'Robinson')"
                Dim pDTTenant As DataTable = mBusiness.ProData.Execute(pSql)
                If pDTTenant.Rows.Count > 0 Then
                    CheckSendingOfData(pLine("FOWSTID").ToString(), pLine("DeviceID").ToString(), pDTTenant.Rows(0)("FOWSTMallName"))
                End If
            Catch ex As Exception
                oError = "An error occured while inserting data: " & ex.Message & ";" & vbCrLf & pIU.GetUpdate("INSERT") & vbCrLf & GetStackTrace(, ex)
                AppendToLog(oError, , mLogFile)
                oError = JsonDictionary.Serialize("Error", oError)
                Return False
            End Try
        Loop

        rZipOut.AddTableCsv(pDtOut, "TenantMonitoringLogs")

        Return True

    End Function

    Public Function ResendReport(ByVal storeid As String, ByVal deviceid As String, ByVal mallname As String) As String
        Dim pOut = CheckSendingOfData(storeid, deviceid, mallname)
        Return pOut
    End Function

    Public Function CheckSendingOfData(ByVal storeid As String, ByVal deviceid As String, ByVal mallname As String) As String

        Dim pSql As String
        Dim clsMallLessorMegaworld As New MallLessorMegaworld
        Dim pDTTenentMonitoring As DataTable
        Dim strLastTransactionID As String
        Dim fowstid As String
        Dim salesdate As String
        Dim pDT As DataTable
        Dim isGenerateNew As Boolean = True
        Dim dtTenantInfo As DataTable
        Dim pOrgID As String = ""
        Dim arrSalesDate As New List(Of String)
        Dim pIU As New InsertUpdate("TenantMonitoringLogs")
        Dim sendfiles As Boolean = False
        'Dim mallname As String = ""
        Dim tenantcode As String = ""
        Dim pFilename As String
        Dim pDrFowst As DataRow

        If mallname = "Robinson" Then
            pSql = "Select * " & vbCrLf &
                " from TenantMonitoringLogs" & vbCrLf &
                " WHERE 1=1 --FileStatus Is NULL" & vbCrLf &
                " And Mall = " & ReadyString(mallname) & vbCrLf &
                " And ISNULL(Reported, 0) = 0" & vbCrLf &
                " And FOWSTID = " & ReadyString(storeid) & vbCrLf &
                " And DeviceID = " & ReadyString(deviceid)
        Else
            pSql = "Select * " & vbCrLf &
                " from TenantMonitoringLogs" & vbCrLf &
                " WHERE FileStatus Is NULL" & vbCrLf &
                " And Mall = " & ReadyString(mallname) & vbCrLf &
                " And ISNULL(Reported, 0) = 0" & vbCrLf &
                " And FOWSTID = " & ReadyString(storeid) & vbCrLf &
                " And DeviceID = " & ReadyString(deviceid)
        End If
        pDTTenentMonitoring = mBusiness.ProData.Execute(pSql)
        'pDTTenentMonitoring.Columns.Add("Filename")

        If pDTTenentMonitoring.Rows.Count > 0 Then
            Dim arrFiles(pDTTenentMonitoring.Rows.Count - 1) As Object
            Dim ctrFiles As Integer = 0
            For Each mall As Object In pDTTenentMonitoring.Rows
                mallname = CStr2(mall("Mall"))
                fowstid = mall("FOWSTID").ToString()
                salesdate = CDate(mall("SalesDate").ToString())
                arrSalesDate.Add(salesdate)

                pSql = "SELECT OrganizationID" & vbCrLf &
                    ",FOWSTMallName" & vbCrLf &
                    ",FtpServer" & vbCrLf &
                    ",FtpUsername" & vbCrLf &
                    ",FtpPassword" & vbCrLf &
                    ",RobinsonTenantId" & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                    " WHERE ID = " & ReadyString(fowstid)
                pDrFowst = mBusiness.ProData.ExecuteRow(pSql)
                If pDrFowst Is Nothing Then
                    Return JsonDictionary.Serialize("Error", "The specified fowst does not exist. ref: vb5RFFS536AM!")
                End If
                pOrgID = CStr2(pDrFowst("OrganizationID"))
                AppendToDebug("OrgID: " & pOrgID & ", FOWSTID: " & fowstid & ", DeviceID: " & deviceid)

                pIU = New InsertUpdate("TenantMonitoringLogs")
                pIU("FileStatus") = ReadyString("creating")
                mBusiness.ProData.Execute(pIU.GetUpdate(" ID = " & ReadyString(mall("ID").ToString())))

                If mallname = "Megaworld" Then
                    dtTenantInfo = clsMallLessorMegaworld.getMegaworldTenantInfo(mBusiness.ProData, fowstid)
                    If (dtTenantInfo.Rows.Count > 0) Then

                        Dim tenantinfo As Object = dtTenantInfo.Rows(0)
                        tenantcode = tenantinfo("TenantCode")
                        arrFiles(ctrFiles) = clsMallLessorMegaworld.GenerateLessorSalesReport(isGenerateNew, mBusiness.ProData, salesdate, tenantinfo, pOrgID, fowstid, deviceid)
                        isGenerateNew = False

                    End If
                    'ElseIf mallname = "Robinson" Then
                    '    pFilename = clsMallLessorMegaworld.GenerateRobinsonReport(mBusiness.ProData, salesdate, pOrgID, fowstid, deviceid)
                    '    mall("Filename") = pFilename
                End If

                ctrFiles += 1
            Next

            If mallname = "Megaworld" And Not String.IsNullOrWhiteSpace(tenantcode) Then
                'For Each files As Object In arrFiles
                '    Dim msg As String = ""
                '    Dim res = FTPMegaworld(mBusiness.ProData, files.DFile, tenantcode, storeid, files.SalesDate, msg)
                '    If msg = "success" Then
                '        res = FTPMegaworld(mBusiness.ProData, files.HFile, tenantcode, storeid, files.SalesDate, msg)
                '        res = FTPMegaworld(mBusiness.ProData, files.SFile, tenantcode, storeid, files.SalesDate, msg)
                '    End If
                'Next
                pIU = New InsertUpdate("TenantMonitoringLogs")
                Dim errmsg As String = ""
                Try
                    'For Each files As Object In arrFiles
                    '    If FTPMegaworldOld(files, tenantcode) Then
                    '        Throw New Exception("FTP error while uploading")
                    '    End If
                    'Next

                    pIU("Reported") = 1
                    pIU("ReportDate") = "getdate()"
                    pIU("FileStatus") = ReadyString("created")
                    pIU("ErrorMessage") = "NULL"
                Catch ex As Exception
                    pIU("FileStatus") = "NULL"
                    pIU("ErrorMessage") = ReadyString(ex.Message)
                    AddLog("Unable to send email to megaworld: " & ex.Message)
                    errmsg = ex.Message
                End Try

                mBusiness.ProData.Execute(pIU.GetUpdate(" FOWSTID = " & ReadyString(storeid) & " And CONVERT(date, SalesDate) IN ('" & String.Join("', '", arrSalesDate) & "')"))
            ElseIf mallname = "Robinson" Then
                Return Nothing ' device will handle this. 2024_07_27__11_04_11_2480
                For Each pDrLog As DataRow In pDTTenentMonitoring.Rows
                    Dim pFtpError As String = ""
                    Dim pFtpServer = CStr2(pDrFowst("FtpServer"))
                    If String.IsNullOrWhiteSpace(pFtpServer) Then
                        pFtpError &= "FTP Server is blank. "
                    End If
                    Dim pFtpUsername = CStr2(pDrFowst("FtpUsername"))
                    If String.IsNullOrWhiteSpace(pFtpUsername) Then
                        pFtpError &= "FTP Username is blank. "
                    End If
                    Dim pFtpPassword = CStr2(pDrFowst("FtpPassword"))
                    If String.IsNullOrWhiteSpace(pFtpPassword) Then
                        pFtpError &= "FTP Password is blank. "
                    End If
                    If pFtpError <> "" Then
                        'pOut.Add("FtpError", pFtpError)
                    End If

                    Dim pSalesDate = CDate(pDrLog("SalesDate"))
                    pFilename = pDrLog("Filename")
                    Dim pFullFilename = Common.ServerPath & "Content\Robinson\" & pOrgID & "\" & fowstid & "\" & deviceid & "\" + pFilename

                    Try
                        'pFtpUsername &= "_wrong"
                        RlcFtpUpload(vFtpServer:=pFtpServer,
                            vFtpUsername:=pFtpUsername,
                            vFtpPassword:=pFtpPassword,
                            vSourceFilename:=pFullFilename,
                            vDestinationFilename:=pFilename)

                        Dim pBatchNum = CInt2(pDrLog("BatchNo"))
                        If pBatchNum < 1 Then
                            pBatchNum = 1
                        Else
                            pBatchNum += 1
                        End If
                        pIU = New InsertUpdate("TenantMonitoringLogs")
                        pIU("Reported") = 1
                        pIU("ReportDate") = "getdate()"
                        pIU("LastUpdated") = "getdate()"
                        pIU("FileStatus") = "'ftp-sent'"
                        pIU("BatchNo") = pBatchNum
                        pSql = pIU.GetUpdate(" FOWSTID = " & ReadyString(fowstid) & " AND DeviceID = " & ReadyString(deviceid) & " AND CONVERT(date, SalesDate) = " & ReadyString(pSalesDate))
                        mBusiness.ProData.ExecuteSQL(pSql)
                    Catch ex As Exception
                        'Return JsonDictionary.Serialize("Error", "An error occured while sending file to FTP server: " + ex.Message)
                        Return JsonDictionary.Serialize("Error", ex.Message)
                    End Try
                Next
            End If

            Return pFilename
        End If
    End Function

    Friend Shared Sub RlcFtpUpload(vFtpServer As String, vFtpUsername As String, vFtpPassword As String,
        vSourceFilename As String, vDestinationFilename As String)

        Const pMaxRetries = 5
        Dim pRetryableErrors = New List(Of String) ' partial match allowed. not case sensitive
        pRetryableErrors.Add("The server response does not contain an SSH identification string.")
        pRetryableErrors.Add("The connection to the remote server was closed before any data was received.")
        'pRetryableErrors.Add("The client is already connected.") ' for debugging only

        Dim pRetries As Integer
        Do
            Try
                If String.IsNullOrWhiteSpace(vDestinationFilename) Then
                    vDestinationFilename = IO.Path.GetFileName(vSourceFilename)
                End If

                Using pClient = New Renci.SshNet.SftpClient(vFtpServer, vFtpUsername, vFtpPassword)
                    pClient.Connect()
                    Using fileStream = System.IO.File.Open(vSourceFilename, FileMode.Open, FileAccess.Read)
                        pClient.UploadFile(fileStream, vDestinationFilename)
                    End Using
                    'pClient.DownloadFile("/documents/document2.docx", Stream)
                    'pClient.Delete("/documents/document1.docx")
                End Using
                Exit Do
            Catch ex As Exception
                AppendToDebug("FTP Error vbETDSE33N3A. No of retries: " & pRetries & vbCrLf & ex.Message)
                If Now.Date = #3/11/2025# Then
                    AppendToDebug("FTP account: vFtpServer: " & vFtpServer & vbCrLf &
                    "vFtpUsername: " & vFtpUsername & vbCrLf &
                    "vFtpPassword: " & vFtpPassword)
                End If
                For Each pRetryableError In pRetryableErrors
                    If ex.Message.ToLower().Contains(pRetryableError.ToLower()) Then
                        If pMaxRetries <= pRetries Then
                            Exit For
                        End If
                        pRetries += 1
                        Continue Do
                    End If
                Next
                Throw
            End Try
        Loop

    End Sub

    Public Shared Function FTPMegaworld(ByVal mProdata As ProData, ByVal filepath As String, ByVal tenantcode As String, ByVal storeid As String, ByVal arrSalesDate As String, ByRef msg As String) As String
        Dim pIU = New InsertUpdate("TenantMonitoringLogs")

        Try
            '// https://learn.microsoft.com/en-us/dotnet/framework/network-programming/how-to-upload-files-with-ftp
            Dim pFtpWebRequest
            Dim filename = Path.GetFileName(filepath)

#If DEBUG Then
            pFtpWebRequest = FtpWebRequest.Create("ftp://localhost/Users/" & filename)
#Else
            pFtpWebRequest = FtpWebRequest.Create("ftp://54.254.44.52/" & tenantcode & "/" & filename)
#End If

            pFtpWebRequest.Method = WebRequestMethods.Ftp.UploadFile

#If DEBUG Then
            pFtpWebRequest.Credentials = New NetworkCredential("rny", "")
#Else
            pFtpWebRequest.Credentials = New NetworkCredential(tenantcode, tenantcode)
#End If

            Using fileStream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read)
                Using requestStream = pFtpWebRequest.GetRequestStream()
                    'fileStream.CopyToAsync(requestStream)
                    fileStream.CopyTo(requestStream)
                    'Using response = pFtpWebRequest.GetResponse() ' stuck dri
                    '    Console.WriteLine("vb3822KY7XDH")
                    'End Using
                End Using
            End Using

            pIU("Reported") = 1
            pIU("ReportDate") = "getdate()"
            pIU("FileStatus") = ReadyString("created")
            AppendToDebug(filename & " - success")
            msg = "success"
        Catch ex As Exception
            pIU("FileStatus") = ReadyString("error")
            pIU("ErrorMessage") = ReadyString(ex.Message)
            AppendToDebug("FTP Error vb2Z34MYWJFM: " & ex.Message & vbCrLf & "(" & filepath & ", " & tenantcode & ")")
            msg = "ex.Message"
        End Try

        mProdata.Execute(pIU.GetUpdate(" FOWSTID = " & ReadyString(storeid) & " AND CONVERT(date, SalesDate) IN ('" & arrSalesDate & "')"))
    End Function

    Public Shared Function FTPMegaworldOld(ByVal filepath As Object, ByVal tenantcode As String)
        Try
            'Dim filereader As StreamReader = New StreamReader(ftpfilepath)
            'Dim filedata As Object = objJSSerializer.Deserialize(Of Object)(filereader.ReadLine)
            'filereader.Close()

            ' Setup session options
            Dim sessionOptions As New SessionOptions

#If DEBUG Then
            With sessionOptions
                .Protocol = Protocol.Ftp
                .HostName = "localhost"
                .UserName = "rny"
                .Password = ""
            End With
#Else
            With sessionOptions
                .Protocol = Protocol.Sftp
                .HostName = "54.254.44.52"
                .UserName = tenantcode
                .Password = tenantcode
                .SshHostKeyFingerprint = "ssh-rsa 2048 4Rp7OzpN3LU0crtMrMf6fMgteMv0j6OdmsxdyCNXvWc=;ssh-rsa 2048 df:72:a7:bc:25:a9:04:ce:1a:0e:d3:10:39:11:d8:3d"
            End With
#End If

            Using session As New Session
                session.DisableVersionCheck = True

                ' Connect
                session.Open(sessionOptions)

                ' Upload files
                Dim transferOptions As New TransferOptions
                transferOptions.TransferMode = TransferMode.Binary

                Dim folder As String = ""

#If DEBUG Then
                folder = "/Users"
#Else
                folder = "/" & tenantcode
#End If

                If Not session.FileExists(folder) Then
                    session.CreateDirectory(folder)
                End If

                Dim transferResult As TransferOperationResult
                transferResult = session.PutFiles(filepath.DFile, folder & "/*.*", False, transferOptions)
                transferResult.Check()

                transferResult = session.PutFiles(filepath.HFile, folder & "/*.*", False, transferOptions)
                transferResult.Check()

                transferResult = session.PutFiles(filepath.SFile, folder & "/*.*", False, transferOptions)
                transferResult.Check()

                'for demo purposes lang man diay ang Zread copy
                'transferResult = session.PutFiles(filepath.ZRead, folder & "/*.*", False, transferOptions)
                'transferResult.Check()

                ' Print results
                'For Each Transfers In transferResult.Transfers
                '    Console.WriteLine("Upload of {0} succeeded", Transfers.FileName)
                'Next
            End Using

            Return False
        Catch ex As Exception
            'Console.WriteLine("Error: {0}", e)
            AppendToDebug("FTP Error: " & ex.Message)
            Return True
        End Try
    End Function

    Private Function CreateBomMakerProductsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        oError = Nothing
        DataImportLog.AddLog("Import from TransBomMakerProducts")
        pCsvReader = New CsvReader(vInputZipFilename, "TransBomMakerProducts")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ParentID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("UnitMeasureID", True, SqlDbType.UniqueIdentifier)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("ParentID")
            .Add("ProductID")
            .Add("Quantity", GetType(Decimal))
            .Add("UnitMeasureID")
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)
            pDtOut.Rows.Add(pDR)
        Loop

        Return pDtOut

    End Function

    Private Function CreateBomMakerContentsDataTable(ByVal vInputZipFilename As String, ByRef oError As String) As DataTable

        Dim pCsvReader As CsvReader
        Dim pBinder As DictionaryBinder
        Dim pDtOut As DataTable
        Dim pDR As DataRow
        Dim pLine As Dictionary(Of String, String)

        oError = Nothing
        DataImportLog.AddLog("Import from TransBomMakerContents")
        pCsvReader = New CsvReader(vInputZipFilename, "TransBomMakerContents")
        pBinder = New DictionaryBinder
        With pBinder.Items
            .Add("ID", True, SqlDbType.UniqueIdentifier)
            .Add("ParentID", True, SqlDbType.UniqueIdentifier)
            .Add("ProductID", True, SqlDbType.UniqueIdentifier)
            .Add("Quantity", True, SqlDbType.Decimal)
            .Add("UnitMeasureID", False, SqlDbType.UniqueIdentifier)

            .Add("UnitMeasure", False, SqlDbType.VarChar)
            .Add("BaseUnitMeasureID", False, SqlDbType.UniqueIdentifier)
            .Add("QtyInBaseUnit", False, SqlDbType.Decimal)
        End With

        pDtOut = New DataTable
        With pDtOut.Columns
            .Add("ID")
            .Add("ParentID")
            .Add("ProductID")
            .Add("Quantity", GetType(Decimal))
            .Add("UnitMeasureID")

            .Add("UnitMeasure")
            .Add("BaseUnitMeasureID")
            .Add("QtyInBaseUnit", GetType(Decimal))
        End With

        Do
            pLine = pCsvReader.ReadLine
            If pLine Is Nothing Then Exit Do
            pBinder.Source = pLine
            If pBinder.Validate(oError) = False Then
                Return Nothing
            End If
            pDR = pBinder.CreateDataRow(pDtOut)
            pDtOut.Rows.Add(pDR)
        Loop

        Return pDtOut

    End Function

    Function GetFranchiseeOrgFOWSTID(ByVal FOWSTID As String) As Boolean
        Dim pSql As String
        Dim dtTemp As DataTable

        pSql = "Select * from MobizLibraryOrganizationFranchisee inner join LibraryFranchiseGroup " & vbCrLf &
                " ON LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
                " WHERE FranchiseeOrgFowstID = " & ReadyString(FOWSTID)

        dtTemp = mBusiness.ProData.Execute(pSql)

        If dtTemp.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function StartTransImport(ByVal vFilename As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pProData As ProData

        If mBusiness1stInstance IsNot Nothing Then
            pProData = mBusiness1stInstance.ProData
        Else
            pProData = mBusiness.ProData
        End If

        pIU = New InsertUpdate("TransImports")
        pIU("ProcessStart") = "GETDATE()"
        pSql = pIU.GetUpdate("Filename = " & ReadyString(vFilename) &
            " AND ProcessStart IS NULL")
        pSql = pProData.ExecuteSQL(pSql)

        Return pSql

    End Function

    Private Function UpdateTransImport(ByVal vFilename As String, ByVal vResult As String, ByVal vSuccess As Boolean) As Integer

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pProData As ProData

        If mBusiness1stInstance IsNot Nothing Then
            pProData = mBusiness1stInstance.ProData
        Else
            pProData = mBusiness.ProData
        End If

        If vResult <> "ok" Then
            Debug.WriteLine("UpdateTransImport not ok")
        End If

        pIU = New InsertUpdate("TransImports")
        pIU("ProcessEnd") = "GETDATE()"
        If String.IsNullOrWhiteSpace(vResult) Then
            pIU("Result") = "NULL"
            pIU("Success") = "NULL"
            pIU("ProcessStart") = "NULL"
            pIU("ProcessEnd") = "NULL"
        Else
            pIU("Result") = ReadyString(vResult)
            pIU("Success") = If(vSuccess, 1, 0)
        End If

        pSql = pIU.GetUpdate("Filename = " & ReadyString(vFilename))
        Try
            pSql = pProData.ExecuteSQL(pSql)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Throw
        End Try

        Return pSql

    End Function

    Private Function InventoryAdjustment(ByVal FOWST As String) As Date
        Dim pSql As String
        Dim dtTemp As DataTable

        'ByVal FOWSTID As String, ByVal ProductID As String, ByVal BinID As String, 
        pSql = "Select top 1 InventoryCountDate from TransInventoryProductCountStock " & vbCrLf &
                "INNER JOIN TransInventoryProductCount " & vbCrLf &
                "ON TransInventoryProductCountStock.InventoryProductCountID = TransInventoryProductCount.ID " & vbCrLf &
                "WHERE " & vbCrLf &
                "TransInventoryProductCount.FowstID = " & ReadyString(FOWST) & vbCrLf &
                "Order by InventoryCountDate desc"

        '"TransInventoryProductCountStock.BinID = " & ReadyString(BinID) & vbCrLf &
        '"AND TransInventoryProductCount.ProductID = " & ReadyString(ProductID) & vbCrLf &


        dtTemp = mBusiness.ProData.Execute(pSql)

        If dtTemp.Rows.Count > 0 Then
            Return CDate(dtTemp.Rows(0).Item("InventoryCountDate"))
        Else
            Return Nothing
        End If

        'If dtTemp.Rows.Count > 0 Then
        '    Select Case TransType
        '        Case "Receiving"
        '            dtTrans.DefaultView.RowFilter = "LastUpdated > " & ReadyString(dtTemp.Rows(0).Item("InventoryCountDate"))
        '        Case "POS"
        '            dtTrans.DefaultView.RowFilter = "InvoiceDate > " & ReadyString(dtTemp.Rows(0).Item("InventoryCountDate"))
        '        Case "Returns" 'since its online, then we dnt have to apply
        '    End Select

        '    Return dtTrans.DefaultView.ToTable
        'End If

    End Function

    Function UpdateZReadCache(mInvoiceID As String) As Boolean
        Dim ZReadDate As String
        Dim pSql As String
        'Dim InvoiceDate As String
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim IsNew As Boolean = False
        Dim dtCurrZRead As DataTable
        Dim dtPrevZRead As DataTable
        Dim dtInvoice As DataTable

        Dim pFOWSTID As String
        Dim pDeviceID As String
        Dim pTransDate As String
        Dim pTransInvoiceNo As String
        Dim dtPOSTransaction As DataTable

        pSql = "Select * from TransInvoice where ID = " & ReadyString(mInvoiceID)

        dtInvoice = mBusiness.ProData.Execute(pSql)

        If dtInvoice.Rows.Count > 0 Then
            pDeviceID = CStr2(dtInvoice.Rows(0).Item("DeviceID"))
            pFOWSTID = CStr2(dtInvoice.Rows(0).Item("FowstID"))
            pTransDate = CStr2(dtInvoice.Rows(0).Item("InvoiceDate"))
            pTransInvoiceNo = CInt2(dtInvoice.Rows(0).Item("InvoiceNumber"))
        Else
            If DeviceID = modClient.SportsTownOrgID Then
                AppendToLog("vbBFFVBY7Z4D! error inside UpdateZReadCache: mInvoiceID: " & mInvoiceID, , "ImportDataClass2")
            End If
            ' 2022_07_16__09_58_31_5048
            If mIsWebPos Then
                Return True ' old code: Return False
            Else
                Return False
            End If
        End If

        dtCurrZRead = GetCurrentZRead(pTransDate, pDeviceID, pFOWSTID)

        dtPOSTransaction = GetPOSTransaction(mInvoiceID, pTransDate, pDeviceID, pFOWSTID)
        ' 2022_07_16__09_58_31_5048
        If mIsWebPos Then
            If dtPOSTransaction.Rows.Count = 0 Then ' arvin's code. mag error sa baba kung dtPOSTransaction.Rows.Count = 0
                Return True
            End If
        End If

        If dtCurrZRead.Rows.Count = 0 Then
            IsNew = True
            dtPrevZRead = GetPreviousZRead(pTransDate, pDeviceID, pFOWSTID)

            If dtPrevZRead.Rows.Count = 0 Then
                'Get ZRead Data from old query
                Dim ReportPOS As New ReportPOS
                Dim dtTempPrevZRead As DataTable

                dtPrevZRead = ReportPOS.getSalesZReportSummary("By Date", CStr2(CDate(pTransDate).AddDays(-1)), pFOWSTID, pDeviceID, mBusiness.ProData)

                If dtPrevZRead.Rows.Count = 0 Then
                    'dtTempPrevZRead.DefaultView.RowFilter = "CurTermNum=" & CStr2(dtPOSTransaction.Rows(0).Item("CurTermNum"))

                    'dtPrevZRead = dtTempPrevZRead.DefaultView(0).DataView.ToTable
                    'dtPrevZRead = dtTempPrevZRead.DesignMode
                    'Else
                    If DeviceID = modClient.SportsTownOrgID Then
                        AppendToLog("vb8HWFV7J3XS! error in UpdateZReadCache: dtPrevZRead.Rows.Count = 0", , "ImportDataClass2")
                    End If
                    Return True
                End If

            End If
        End If

        Dim p As New InsertUpdate("TransZReadCache")

        If IsNew Then
            p("DeviceID") = ReadyString(pDeviceID)
            p("FOWSTID") = ReadyString(pFOWSTID)

            If CDec2(dtPrevZRead.Rows(0).Item("PresentationPrevGrossSales")) = 0 Then
                p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
            Else
                p("PresentationPrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PresentationPrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("PresentationCurrGrossSales"))
            End If
            p("PrevGrossSales") = CDec2(dtPrevZRead.Rows(0).Item("PrevGrossSales")) + CDec2(dtPrevZRead.Rows(0).Item("CurGrossSales"))
            p("PrevTotalDiscount") = CDec2(dtPrevZRead.Rows(0).Item("PrevTotalDiscount")) + CDec2(dtPrevZRead.Rows(0).Item("CurTotalDiscount"))
            p("PrevSalesReturnRefund") = CDec2(dtPrevZRead.Rows(0).Item("PrevSalesReturnRefund")) + CDec2(dtPrevZRead.Rows(0).Item("CurSalesReturnRefund"))
            p("PrevSalesReturnExchange") = CDec2(dtPrevZRead.Rows(0).Item("PrevSalesReturnExchange")) + CDec2(dtPrevZRead.Rows(0).Item("CurSalesReturnExchange"))
            p("PrevTotalVAT") = CDec2(dtPrevZRead.Rows(0).Item("PrevTotalVAT")) + CDec2(dtPrevZRead.Rows(0).Item("CurTotalVAT"))
            p("PrevServiceCharge") = CDec2(dtPrevZRead.Rows(0).Item("PrevServiceCharge")) + CDec2(dtPrevZRead.Rows(0).Item("CurServiceCharge"))
            p("PrevChargeAmount") = CDec2(dtPrevZRead.Rows(0).Item("PrevChargeAmount")) + CDec2(dtPrevZRead.Rows(0).Item("ChargeAmount"))
            p("PrevPhNatTeamDiscount") = CDec2(dtPrevZRead.Rows(0).Item("PrevPhNatTeamDiscount")) + CDec2(dtPrevZRead.Rows(0).Item("PrevPhNatTeamDiscount"))
            p("PrevSoloParentDiscount") = CDec2(dtPrevZRead.Rows(0).Item("PrevSoloParentDiscount")) + CDec2(dtPrevZRead.Rows(0).Item("PrevSoloParentDiscount"))
            p("PrevSCDiscount") = CDec2(dtPrevZRead.Rows(0).Item("PrevSCDiscount")) + CDec2(dtPrevZRead.Rows(0).Item("CurSCDiscount"))
            p("PrevSCDiscountExempt") = CDec2(dtPrevZRead.Rows(0).Item("PrevSCDiscountExempt")) + CDec2(dtPrevZRead.Rows(0).Item("CurSCDiscountExempt"))
            p("PrevVATExempt") = CDec2(dtPrevZRead.Rows(0).Item("PrevVATExempt")) + CDec2(dtPrevZRead.Rows(0).Item("CurVATExempt"))

            p("SalesDate") = ReadyString(pTransDate)
            p("PresentationCurrGrossSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
            p("CurGrossSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
            p("CurTotalDiscount") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalDiscount"))
            p("CurPhNatTeamDiscount") = CDec2(dtPOSTransaction.Rows(0).Item("CurPhNatTeamDiscount"))
            p("CurSoloParentDiscount") = CDec2(dtPOSTransaction.Rows(0).Item("CurSoloParentDiscount"))
            p("CurSCDiscount") = CDec2(dtPOSTransaction.Rows(0).Item("CurSCDiscount"))
            p("CurSCDiscountExempt") = CDec2(dtPOSTransaction.Rows(0).Item("CurSCDiscountExempt"))
            p("CurTotalVATSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVATSales"))
            p("CurTotalVAT") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVAT"))
            p("CurVATExempt") = CDec2(dtPOSTransaction.Rows(0).Item("CurVATExempt"))
            p("CurLocalTax") = CDec2(dtPOSTransaction.Rows(0).Item("CurLocalTax"))
            p("CurServiceCharge") = CDec2(dtPOSTransaction.Rows(0).Item("CurServiceCharge"))
            p("CurNoTAXSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurNoTAXSales"))
            p("CurVATExemptSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurVATExemptSales"))
            p("CurZeroRatedVATSales") = CDec2(dtPOSTransaction.Rows(0).Item("CurZeroRatedVATSales"))
            p("CurOthers") = CDec2(dtPOSTransaction.Rows(0).Item("CurOthers"))
            p("CashAmount") = CDec2(dtPOSTransaction.Rows(0).Item("CashAmount"))
            p("DebitCardAmount") = CDec2(dtPOSTransaction.Rows(0).Item("DebitCardAmount"))
            p("CreditCardAmount") = CDec2(dtPOSTransaction.Rows(0).Item("CreditCardAmount"))
            p("SRVAmount") = CDec2(dtPOSTransaction.Rows(0).Item("SRVAmount"))
            p("SMACAmount") = CDec2(dtPOSTransaction.Rows(0).Item("SMACAmount"))
            p("CheckPaymentAmount") = CDec2(dtPOSTransaction.Rows(0).Item("CheckPaymentAmount"))
            p("OtherTypeAmount") = CDec2(dtPOSTransaction.Rows(0).Item("OtherTypeAmount"))
            p("OnCreditAmount") = CDec2(dtPOSTransaction.Rows(0).Item("OnCreditAmount"))
            p("ChargeAmount") = CDec2(dtPOSTransaction.Rows(0).Item("ChargeAmount"))
            p("EPlusAmount") = CDec2(dtPOSTransaction.Rows(0).Item("EPlusAmount"))
            p("VoucherAmount") = CDec2(dtPOSTransaction.Rows(0).Item("VoucherAmount"))
            p("CurBegInvoiceNo") = pTransInvoiceNo
            p("CurEndInvoiceNo") = pTransInvoiceNo
            p("CurTransCountInv") = 1 'CDec2(dtPrevZRead.Rows(0).Item("CurTransCountInv") + 1)
            p("CurTransInvItemCount") = CDec2(dtPOSTransaction.Rows(0).Item("CurTransInvItemCount")) '+ CDec2(dtPrevZRead.Rows(0).Item("CurTransInvItemCount"))
            p("CurTransCountMemoReturnExchange") = 0
            p("CurSalesReturnExchangeItemCount") = 0
            p("CurTransCountMemoRefund") = 0
            p("CurSalesRefundItemCount") = 0
            p("CurTermNum") = CDec2(dtPrevZRead.Rows(0).Item("CurTermNum"))
            p("FowstNo") = CDec2(dtPrevZRead.Rows(0).Item("FowstNo"))
            p("CurTotalVatablePortionOfSCDTransaction") = CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction"))
            p("DiscountRefund") = CDec2(dtPOSTransaction.Rows(0).Item("DiscountRefund"))
        Else
            p("PresentationCurrGrossSales") = CDec2(dtCurrZRead.Rows(0).Item("PresentationCurrGrossSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
            p("CurGrossSales") = CDec2(dtCurrZRead.Rows(0).Item("CurGrossSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurGrossSales"))
            p("CurTotalDiscount") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalDiscount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalDiscount"))
            p("CurPhNatTeamDiscount") = CDec2(dtCurrZRead.Rows(0).Item("CurPhNatTeamDiscount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurPhNatTeamDiscount"))
            p("CurSoloParentDiscount") = CDec2(dtCurrZRead.Rows(0).Item("CurSoloParentDiscount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurSoloParentDiscount"))
            p("CurSCDiscount") = CDec2(dtCurrZRead.Rows(0).Item("CurSCDiscount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurSCDiscount"))
            p("CurSCDiscountExempt") = CDec2(dtCurrZRead.Rows(0).Item("CurSCDiscountExempt")) + CDec2(dtPOSTransaction.Rows(0).Item("CurSCDiscountExempt"))
            p("CurTotalVATSales") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVATSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVATSales"))
            p("CurTotalVAT") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVAT")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVAT"))
            p("CurVATExempt") = CDec2(dtCurrZRead.Rows(0).Item("CurVATExempt")) + CDec2(dtPOSTransaction.Rows(0).Item("CurVATExempt"))
            p("CurLocalTax") = CDec2(dtCurrZRead.Rows(0).Item("CurLocalTax")) + CDec2(dtPOSTransaction.Rows(0).Item("CurLocalTax"))
            p("CurServiceCharge") = CDec2(dtCurrZRead.Rows(0).Item("CurServiceCharge")) + CDec2(dtPOSTransaction.Rows(0).Item("CurServiceCharge"))
            p("CurNoTAXSales") = CDec2(dtCurrZRead.Rows(0).Item("CurNoTAXSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurNoTAXSales"))
            p("CurVATExemptSales") = CDec2(dtCurrZRead.Rows(0).Item("CurVATExemptSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurVATExemptSales"))
            p("CurZeroRatedVATSales") = CDec2(dtCurrZRead.Rows(0).Item("CurZeroRatedVATSales")) + CDec2(dtPOSTransaction.Rows(0).Item("CurZeroRatedVATSales"))
            p("CurOthers") = CDec2(dtCurrZRead.Rows(0).Item("CurOthers")) + CDec2(dtPOSTransaction.Rows(0).Item("CurOthers"))
            p("CashAmount") = CDec2(dtCurrZRead.Rows(0).Item("CashAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("CashAmount"))
            p("DebitCardAmount") = CDec2(dtCurrZRead.Rows(0).Item("DebitCardAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("DebitCardAmount"))
            p("CreditCardAmount") = CDec2(dtCurrZRead.Rows(0).Item("CreditCardAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("CreditCardAmount"))
            p("SRVAmount") = CDec2(dtCurrZRead.Rows(0).Item("SRVAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("SRVAmount"))
            p("SMACAmount") = CDec2(dtCurrZRead.Rows(0).Item("SMACAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("SMACAmount"))
            p("CheckPaymentAmount") = CDec2(dtCurrZRead.Rows(0).Item("CheckPaymentAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("CheckPaymentAmount"))
            p("OtherTypeAmount") = CDec2(dtCurrZRead.Rows(0).Item("OtherTypeAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("OtherTypeAmount"))
            p("OnCreditAmount") = CDec2(dtCurrZRead.Rows(0).Item("OnCreditAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("OnCreditAmount"))
            p("ChargeAmount") = CDec2(dtCurrZRead.Rows(0).Item("ChargeAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("ChargeAmount"))
            p("EPlusAmount") = CDec2(dtCurrZRead.Rows(0).Item("EPlusAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("EPlusAmount"))
            p("VoucherAmount") = CDec2(dtCurrZRead.Rows(0).Item("VoucherAmount")) + CDec2(dtPOSTransaction.Rows(0).Item("VoucherAmount"))
            p("CurTotalVatablePortionOfSCDTransaction") = CDec2(dtCurrZRead.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTotalVatablePortionOfSCDTransaction"))
            p("DiscountRefund") = CDec2(dtPOSTransaction.Rows(0).Item("DiscountRefund"))

            'mag zero ni sya kung sales return ang first transaction of the day
            If CDec2(dtCurrZRead.Rows(0).Item("CurBegInvoiceNo")) = 0 Then
                p("CurBegInvoiceNo") = pTransInvoiceNo
            End If

            p("CurEndInvoiceNo") = pTransInvoiceNo

            p("CurTransCountInv") = CInt2(dtCurrZRead.Rows(0).Item("CurTransCountInv")) + 1
            p("CurTransInvItemCount") = CDec2(dtCurrZRead.Rows(0).Item("CurTransInvItemCount")) + CDec2(dtPOSTransaction.Rows(0).Item("CurTransInvItemCount"))

        End If

        If IsNew Then
            pSql = p.GetInsert()
        Else
            pSql = p.GetUpdate("ID = " & ReadyString(dtCurrZRead.Rows(0).Item("ID")))
        End If

        mBusiness.ProData.Execute(pSql)

        Return True
    End Function

    Function UpdateEJournalCache(mInvoiceID As String) As Boolean
        Dim pSql As String
        Dim dtInvoiceDetails As DataTable
        Dim dtInvoiceHeader As DataTable

        pSql = "Select * from TransInvoice where ID = " & ReadyString(mInvoiceID)
        dtInvoiceHeader = mBusiness.ProData.Execute(pSql)

        dtInvoiceDetails = GetInvoiceDetailsForEJournal(mInvoiceID)

        If dtInvoiceDetails.Rows.Count > 0 Then
            For i As Integer = 0 To dtInvoiceDetails.Rows.Count - 1

                Dim p As New InsertUpdate("TransSalesInvoiceAndRebatesCache")

                p("FOWSTID") = ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("FOWSTID")))
                p("DeviceID") = ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("DeviceID")))
                p("TransactionID") = ReadyString(CStr2(dtInvoiceDetails.Rows(0).Item("ID")))

                p("TransactionType") = ReadyString("Invoice")

                p("Customer") = ReadyString(dtInvoiceDetails.Rows(i).Item("Customer"))
                p("CashierName") = ReadyString(dtInvoiceDetails.Rows(i).Item("CashierName"))
                p("ProductNo") = ReadyString(dtInvoiceDetails.Rows(i).Item("ProductNo"))
                p("ProductName") = ReadyString(dtInvoiceDetails.Rows(i).Item("ProductName"))
                p("TransactionNo") = ReadyString(dtInvoiceDetails.Rows(i).Item("TransactionNo"))
                p("TransactionDate") = ReadyString(dtInvoiceDetails.Rows(i).Item("TransactionDate"))
                p("Quantity") = CDbl(dtInvoiceDetails.Rows(i).Item("Quantity"))
                p("UnitMeasure") = ReadyString(dtInvoiceDetails.Rows(i).Item("UnitMeasure"))
                p("UnitPrice") = CDbl(dtInvoiceDetails.Rows(i).Item("UnitPrice"))
                p("SubTotal") = CDbl(dtInvoiceDetails.Rows(i).Item("SubTotal"))
                p("DiscountAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("DiscountAmount"))
                p("DiscountedUnitPrice") = CDbl(dtInvoiceDetails.Rows(i).Item("DiscountedUnitPrice"))
                p("DiscountedSubTotal") = CDbl(dtInvoiceDetails.Rows(i).Item("DiscountedSubTotal"))
                p("CashAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("CashAmount"))
                p("CheckPaymentAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("CheckPaymentAmount"))
                p("DebitCardAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("DebitCardAmount"))
                p("CreditCardAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("CreditCardAmount"))
                p("SRVAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("SRVAmount"))
                p("OnCreditAmount") = CDec2((dtInvoiceDetails.Rows(i).Item("OnCreditAmount")))
                p("OtherTypeAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("OtherTypeAmount"))
                p("VATPercent") = CDbl(dtInvoiceDetails.Rows(i).Item("VATPercent"))
                p("VatableSales") = CDbl(dtInvoiceDetails.Rows(i).Item("VatableSales"))
                p("VATAmount") = CDbl(dtInvoiceDetails.Rows(i).Item("VATAmount"))
                p("CustomerChangeCash") = CDbl(dtInvoiceDetails.Rows(i).Item("CustomerChangeCash"))

                pSql = p.GetInsert()

                mBusiness.ProData.Execute(pSql)

            Next

            Dim totalSales As Double
            Dim mMonthYear As Date
            Dim MinInvoiceNo As Integer
            Dim dtIsCurrentRowExist As DataTable

            pSql = "Select SUM(SubTotal) from TransSalesInvoiceAndRebatesCache " & vbCrLf &
                "WHERE convert(date, TransactionDate) = " & ReadyString(dtInvoiceDetails.Rows(0).Item("TransactionDate")) & vbCrLf &
                " AND FOWSTID = " & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("FOWSTID"))) & vbCrLf &
                " AND DeviceID=" & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("DeviceID"))) & vbCrLf &
                " AND TransactionType = 'Invoice'"

            totalSales = CDec2(mBusiness.ProData.ExecuteScalar(pSql))

            mMonthYear = CDate(dtInvoiceDetails.Rows(0).Item("TransactionDate"))


            pSql = "Select * from TransSalesInvoiceAndRebatesHeader " & vbCrLf &
                 " WHERE Month(TransactionMonthYear)= " & mMonthYear.Month & vbCrLf &
                  " AND Year(TransactionMonthYear)= " & mMonthYear.Year & vbCrLf &
                " AND FOWSTID = " & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("FOWSTID"))) & vbCrLf &
                " AND DeviceID=" & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("DeviceID")))

            dtIsCurrentRowExist = mBusiness.ProData.Execute(pSql)

            If dtIsCurrentRowExist.Rows.Count = 0 Then
                pSql = "Select MIN(TransactionNo) from TransSalesInvoiceAndRebatesCache " & vbCrLf &
               "WHERE convert(date, TransactionDate) = " & ReadyString(dtInvoiceDetails.Rows(0).Item("TransactionDate")) & vbCrLf &
               " AND FOWSTID = " & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("FOWSTID"))) & vbCrLf &
               " AND DeviceID=" & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("DeviceID"))) & vbCrLf &
               " AND TransactionType = 'Invoice'"

                MinInvoiceNo = CInt2(mBusiness.ProData.ExecuteScalar(pSql))

                pSql = "INSERT INTO TransSalesInvoiceAndRebatesHeader(" & vbCrLf &
                        "DeviceID, FOWSTID, TransactionMonthYear, SIFrom, SITo, OriginalValue)  VALUES(" & vbCrLf &
                        ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("DeviceID"))) & "," & vbCrLf &
                        ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("FOWSTID"))) & "," & vbCrLf &
                        ReadyString(dtInvoiceDetails.Rows(0).Item("TransactionDate")) & "," & vbCrLf &
                        MinInvoiceNo & "," & vbCrLf &
                        CInt2(dtInvoiceDetails.Rows(0).Item("TransactionNo")) & "," & vbCrLf &
                        totalSales & ")"
            Else
                pSql = "Update TransSalesInvoiceAndRebatesHeader SET " & vbCrLf &
                    "OriginalValue = " & totalSales & "," & vbCrLf &
                    "SITo = " & CInt2(dtInvoiceDetails.Rows(0).Item("TransactionNo")) & vbCrLf &
                  " WHERE Month(TransactionMonthYear)= " & mMonthYear.Month & vbCrLf &
                  " AND Year(TransactionMonthYear)= " & mMonthYear.Year & vbCrLf &
                  " AND DeviceID = " & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("DeviceID"))) & vbCrLf &
                  " AND FOWSTID = " & ReadyString(CStr2(dtInvoiceHeader.Rows(0).Item("FOWSTID")))

            End If

            mBusiness.ProData.Execute(pSql)
        End If

        Return True

    End Function

    Function GetInvoiceDetailsForEJournal(mInvoiceID As String)
        Dim pSql As String

        pSql = "SELECT " &
                " invlgr.TransactionID as ID " &
                " 	,prd.ProductNo " &
                " 	,prd.ProductName " &
                " 	,inv.InvoiceNumber as TransactionNo " &
                " 	,inv.InvoiceDate as TransactionDate " &
                " 	,cust.CompanyName as Customer" &
                " 	,invitem.Quantity " &
                " 	,invitem.UnitMeasure " &
                " 	,invitem.UnitPrice " &
                " 	,invitem.SubTotal " &
                " 	,invitem.DiscountAmount " &
                " 	,invitem.DiscountedUnitPrice " &
                " 	,invitem.Quantity * invitem.DiscountedUnitPrice as DiscountedSubTotal " &
                " 	,invitem.TaxPercent as VATPercent " &
                " 	,invitem.TaxSales - invitem.DiscountSales as VATableSales " &
                " 	,invitem.TaxAmount - invitem.DiscountTax as VATAmount " &
                " 	,ISNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) + ', ',', '), '') +  " &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) + ' ',' '), '') +  " &
                " 	 ISNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) + ' ',' '), '') as CashierName " &
                "   , custpay.CashAmount " &
                "   , custpay.CheckPaymentAmount " &
                "   , custpay.DebitCardAmount " &
                "   , custpay.CreditCardAmount " &
                "   , custpay.SRVAmount " &
                "   , custpay.OnCreditAmount " &
                "   , custpay.OtherTypeAmount " &
                "   , custpay.CustomerChangeCash "

        'pSql = pSql & strProductColumns

        '" INTO #tbl1 " & vbCrLf &

        pSql = pSql &
                " FROM TransInventoryLedger invlgr  " &
                " LEFT JOIN LibraryFowstBin fbin ON (fbin.ID = invlgr.BinID) " &
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (fowst.ID = invlgr.FOWSTID) " & vbCrLf


        pSql &= " JOIN (" & vbCrLf &
            "           SELECT TransInvoiceItems.ID" & vbCrLf &
            "           ,TransInvoiceItems.InvoiceID" & vbCrLf &
            "           ,TransInvoiceItemsRebates.ProductID" & vbCrLf &
            "           ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity AS Quantity" & vbCrLf &
            "           ,TransInvoiceItemsRebates.UnitMeasure" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Amount * -1 AS UnitPrice" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Amount * TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity * -1 AS SubTotal" & vbCrLf &
            "           ,0 AS DiscountAmount" & vbCrLf &
            "           ,TransInvoiceItemsRebates.Amount * -1 AS DiscountedUnitPrice" & vbCrLf &
            "           ,0 AS TaxPercent" & vbCrLf &
            "           ,0 AS TaxSales" & vbCrLf &
            "           ,0 AS DiscountSales" & vbCrLf &
            "           ,0 AS TaxAmount" & vbCrLf &
            "           ,0 AS DiscountTax" & vbCrLf &
            "           ,0 AS Cost" & vbCrLf &
            "           ,0 AS COGS" & vbCrLf &
            "           ,0 AS UnitGP" & vbCrLf &
            "           ,0 AS UnitGPM" & vbCrLf &
            "           ,0 AS LineGP" & vbCrLf &
            "           ,0 AS LineGPM" & vbCrLf &
            "           ,0 AS LineCost" & vbCrLf &
            "           ,0 AS GrossSalesNetOfVATExemptions" & vbCrLf &
            "           ,'Returned for Rebate' AS TransactionType" & vbCrLf &
            " 	        FROM TransInvoiceItems" & vbCrLf &
            "           JOIN TransInvoiceItemsRebates ON TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID" & vbCrLf &
            "           UNION" & vbCrLf &
            " 		    SELECT TransInvoiceItems.ID" & vbCrLf &
            "           ,TransInvoiceItems.InvoiceID" & vbCrLf &
            "        	,TransInvoiceItems.ProductID" & vbCrLf &
            "           ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            "           ,TransInvoiceItems.Quantity" & vbCrLf &
            "           ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "           ,TransInvoiceItems.UnitPrice" & vbCrLf &
            "           ,TransInvoiceItems.SubTotal" & vbCrLf &
            "           ,TransInvoiceItems.DiscountAmount" & vbCrLf &
            "           ,TransInvoiceItems.DiscountedUnitPrice" & vbCrLf &
            "           ,TransInvoiceItems.TaxPercent" & vbCrLf &
            "           ,TransInvoiceItems.TaxSales" & vbCrLf &
            "           ,TransInvoiceItems.DiscountSales" & vbCrLf &
            "           ,TransInvoiceItems.TaxAmount" & vbCrLf &
            "           ,TransInvoiceItems.DiscountTax" & vbCrLf &
            "           ,TransInvoiceItems.Cost" & vbCrLf &
            "           ,TransInvoiceItems.COGS" & vbCrLf &
            "           ,TransInvoiceItems.UnitGP" & vbCrLf &
            "           ,TransInvoiceItems.UnitGPM" & vbCrLf &
            "           ,TransInvoiceItems.LineGP" & vbCrLf &
            "           ,TransInvoiceItems.LineGPM" & vbCrLf &
            "           ,TransInvoiceItems.LineCost" & vbCrLf &
            "           ,TransInvoiceItems.GrossSalesNetOfVATExemptions" & vbCrLf &
            "           ,NULL AS TransactionType" & vbCrLf &
            " 		    FROM TransInvoiceItems" & vbCrLf &
            "           )invitem ON invitem.InvoiceID = invlgr.TransactionID" & vbCrLf &
            "           AND invitem.ProductID = invlgr.ProductID" & vbCrLf


        pSql &= " INNER JOIN TransInvoice inv ON (invitem.InvoiceID = inv.ID) " &
                        " INNER JOIN MobizLibraryOrganization cust ON (cust.ID = inv.CustomerID) " &
                        " LEFT JOIN TransCustomerPaymentInvoices custpayinv ON (custpayinv.InvoiceID = inv.ID) " &
                        " LEFT JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                        " LEFT JOIN MobizLibraryOrganizationFOWST custbill ON (custbill.ID = inv.BillingAddressID) " &
                        " LEFT JOIN MobizLibraryCities custbillcty ON (custbill.FowstCityID = custbillcty.ID)  " &
                        " LEFT JOIN MobizLibraryCountries custbillctry ON (custbill.FowstCountryID = custbillctry.ID)  " &
                        " LEFT JOIN MobizLibraryOrganizationFOWST custdel ON (custdel.ID = inv.DeliveryAddressID) " &
                        " LEFT JOIN MobizLibraryCities custdelcty ON (custdel.FowstCityID = custdelcty.ID)  " &
                        " LEFT JOIN MobizLibraryCountries custdelctry ON (custdel.FowstCountryID = custdelctry.ID)  " &
                        " INNER JOIN LibraryDevice dev ON (dev.ID = inv.DeviceID) " &
                        " LEFT JOIN MobizLibraryPeople cshr ON (cshr.ID = custpay.UserID) " &
                        " INNER JOIN MobizLibraryPIMProduct prd ON (prd.ID = invlgr.ProductID) " &
                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures001 prd001 ON (prd.ID = prd001.ProductID) " &
                        " LEFT JOIN MobizLibraryPIMProductVariableFeatures002 prd002 ON (prd.ID = prd002.ProductID) " &
                        " INNER JOIN MobizLibraryPIMPriceLevel plvl ON (plvl.ID = invitem.PriceLevelID) " &
                        " WHERE invlgr.TransactionID = " & ReadyString(mInvoiceID)

        Return mBusiness.ProData.Execute(pSql)
    End Function

    Function GetCurrentZRead(mDate As String, pDeviceID As String, pFOWSTID As String) As DataTable
        Dim pSql As String

        pSql = "Select * FROM TransZReadCache " & vbCrLf &
                " WHERE convert(date, SalesDate) = " & ReadyString(mDate) & vbCrLf &
                " AND FOWSTID = " & ReadyString(pFOWSTID) & vbCrLf &
                " AND DeviceID = " & ReadyString(pDeviceID)

        Return mBusiness.ProData.Execute(pSql)
    End Function

    Function GetPreviousZRead(mDate As String, pDeviceID As String, pFOWSTID As String) As DataTable
        Dim pSql As String

        pSql = "Select top 1 * FROM TransZReadCache " & vbCrLf &
                " WHERE convert(date, SalesDate) < " & ReadyString(mDate) & vbCrLf &
                " AND FOWSTID = " & ReadyString(pFOWSTID) & vbCrLf &
                " AND DeviceID = " & ReadyString(pDeviceID) & vbCrLf &
                " Order by SalesDate desc "

        Return mBusiness.ProData.Execute(pSql)
    End Function

    Function GetPOSTransaction(mTransactionID As String, mDate As String, mDevice As String, mFOWST As String) As DataTable
        Dim pFinder As New Finder
        Dim pSql As String

        pFinder = New Finder

        With pFinder

            .ProData = mBusiness.ProData

            With .Fields
                With .Add("CurGrossSales", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'" +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurTotalDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(custpay.DiscountAmount) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID " & vbCrLf &
                                " AND disc.DiscountTypeID NOT IN ('055a0a33-dd98-47ec-add3-8b2009f3944a', '34a4032f-a142-4c15-a868-4ac4410434df') -- Senior, PWD " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND disc.NoOfSeniorCitizens IS NULL " +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurSoloParentDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SoloParentDiscount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.ID = " & ReadyString(mTransactionID) &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " & ReadyString((mDate)) + " AND " + ReadyString((mDate)) &
                               " AND (LibraryDiscountSettings.DiscountType = 'Solo Parent Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("CurPhNatTeamDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.PhNatTeamDiscount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.ID = " & ReadyString(mTransactionID) &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " & ReadyString((mDate)) + " AND " + ReadyString((mDate)) &
                               " AND (LibraryDiscountSettings.DiscountType = 'Philippine National Sports Team Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("CurSCDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscount)" &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN TransInvoiceDiscount as disc ON disc.InvoiceID = inv.ID  " & vbCrLf &
                               " JOIN LibraryDiscountSettings ON LibraryDiscountSettings.ID = disc.DiscountTypeID  " & vbCrLf &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.ID = " & ReadyString(mTransactionID) &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " & ReadyString((mDate)) + " AND " + ReadyString((mDate)) &
                               " AND (LibraryDiscountSettings.DiscountType = 'Senior Citizen Discount' " & vbCrLf &
                               " OR LibraryDiscountSettings.DiscountType = 'Person With Disability Discount') " & vbCrLf &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("CurVATExempt", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.ForegoneVAT) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' " &
                               " AND inv.ID = " + ReadyString(mTransactionID) &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("CurSCDiscountExempt", SqlDbType.Money)
                    '.Expression = "(case when [SCDiscount]>(0) AND [InvoiceNumber]>(0) then [VAT]+[Vatable] else (0) end)"
                    .Expression = " ISNULL((SELECT SUM(inv.VAT) + SUM(Vatable) " &
                                " FROM TransInvoice inv " &
                                " WHERE inv.InvoiceNumber > 0 AND inv.SCDiscount > 0" &
                                " AND inv.ID = " + ReadyString(mTransactionID) &
                                " AND CONVERT(date, inv.InvoiceDate) BETWEEN " & ReadyString((mDate)) & " AND " & ReadyString((mDate)) &
                                " AND inv.FOWSTID = devf.FowstID " &
                                " AND inv.DeviceID = devf.DeviceID) " &
                                ",0)"
                End With

                With .Add("RegularDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                               " FROM TransInvoiceItems invitem " +
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                               " WHERE inv.InvoiceNumber > 0 " +
                                               " AND inv.ID = " + ReadyString(mTransactionID) +
                                               " AND invdisc.DiscountTypeID = '764EDACB-0131-4033-8E44-7B903368008C' " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("EmployeeDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                               " FROM TransInvoiceItems invitem " +
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                               " WHERE inv.InvoiceNumber > 0 " +
                                               " AND inv.ID = " + ReadyString(mTransactionID) +
                                               " AND invdisc.DiscountTypeID = '7804A35A-B531-4380-945E-FC9643E99843' " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"

                End With

                With .Add("SeniorCitizenDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                                               " FROM TransInvoiceItems invitem " +
                                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                                               " WHERE inv.InvoiceNumber > 0 " +
                                                               " AND inv.ID = " + ReadyString(mTransactionID) +
                                                               " AND invdisc.DiscountTypeID = '055A0A33-DD98-47EC-ADD3-8B2009F3944A' " +
                                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                                               " AND inv.FOWSTID = devf.FowstID " +
                                               " AND inv.DeviceID = devf.DeviceID) " +
                                               ",0)"
                End With

                With .Add("PWDDiscount", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                               " FROM TransInvoiceItems invitem " +
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                               " WHERE inv.InvoiceNumber > 0 " +
                                               " AND inv.ID = " + ReadyString(mTransactionID) +
                                               " AND invdisc.DiscountTypeID = '34A4032F-A142-4C15-A868-4AC4410434DF' " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"

                End With

                With .Add("VIPDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                               " FROM TransInvoiceItems invitem " +
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                               " WHERE inv.InvoiceNumber > 0 " +
                                               " AND inv.ID = " + ReadyString(mTransactionID) +
                                               " AND invdisc.DiscountTypeID = '35F954A0-C67C-499C-AF60-F08E64477FDC' " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("GPCDiscount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                            " FROM TransInvoiceItems invitem " +
                                            " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                            " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                            " WHERE inv.InvoiceNumber > 0 " +
                                            " AND inv.ID = " + ReadyString(mTransactionID) +
                                            " AND invdisc.DiscountTypeID = 'C2218C86-0450-4CD3-B019-3EDD43E51741' " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("Discount1", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(invitem.DiscountAmount) " +
                                               " FROM TransInvoiceItems invitem " +
                                               " INNER JOIN TransInvoice inv ON (inv.ID = invitem.InvoiceID) " +
                                               " INNER JOIN TransInvoiceDiscount invdisc ON (invitem.InvoiceID = invdisc.InvoiceID) " +
                                               " WHERE inv.InvoiceNumber > 0 " +
                                               " AND inv.ID = " + ReadyString(mTransactionID) +
                                               " AND invdisc.DiscountTypeID = 'C4199347-6A00-45C9-BE11-8934DD63ECD1' " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurTotalVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxSales,0)- ISNULL(invitem.DiscountSales,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL)" &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurTotalVAT", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(invitem.TaxAmount,0)- ISNULL(invitem.DiscountTax,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' AND (convert(int,inv.SCDiscount)=0 OR inv.SCDiscount IS NULL)" &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurLocalTax", SqlDbType.Money)
                    .Expression = "0"
                End With

                With .Add("CurServiceCharge", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(srledger.Fees, 0) + ISNULL(srledger.RefundFees, 0)) " +
                               " FROM TransSalesReturnLedger srledger " +
                               " INNER JOIN TransMemo memo ON (srledger.SourceMemoID = memo.ID) " +
                               " WHERE convert(date,memo.LastUpdated) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND memo.FOWSTID = devf.FowstID " +
                               " AND memo.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurNoTAXSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.NonVATSale) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                               " WHERE convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CurVATExemptSales", SqlDbType.Money)
                    '.Expression = "ISNULL((SELECT SUM(invitem.GrossSalesNetOfVATExemptions - invitem.DiscountAmount) " +
                    '           " FROM TransCustomerPaymentInvoices custpayinv " +
                    '           " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                    '           " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                    '           " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                    '           " WHERE (inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF' ) " +
                    '           " AND invitem.TaxAmount = 0 AND ISNULL(invitem.VATableSale,0) = 0 " +
                    '           " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                    '           " AND inv.ID = " + ReadyString(mTransactionID) +
                    '           " AND inv.FOWSTID = devf.FowstID " +
                    '           " AND inv.DeviceID = devf.DeviceID) " +
                    '           ",0)"
                    .Expression = "ISNULL((SELECT SUM(inv.VATExempt - inv.SCDiscount) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " AND convert(date,inv.InvoiceDate) BETWEEN " & ReadyString((mDate)) & " AND " & ReadyString((mDate)) &
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("CurZeroRatedVATSales", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(invitem.ZeroRatedSale) " &
                               " FROM dbo.TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN dbo.TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN dbo.TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " INNER JOIN dbo.TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " &
                               " WHERE convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("CurOthers", SqlDbType.Money)
                    .Expression = "0"
                End With

                With .Add("CashAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CashAmount,0) - ISNULL(custpay.CustomerChangeCash,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("DebitCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.DebitCardAmount,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CreditCardAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CreditCardAmount,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("CheckPaymentAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.CheckPaymentAmount,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("SRVAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.SRVAmount,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("OtherTypeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OtherTypeAmount,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("OnCreditAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.OnCreditAmount,0)) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID) " +
                               ",0)"
                End With

                With .Add("ChargeAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(custpay.ChargeAmount,0)) " +
                              " FROM TransCustomerPaymentInvoices custpayinv " +
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                              " WHERE convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                              " AND inv.ID = " + ReadyString(mTransactionID) +
                              " AND inv.FOWSTID = devf.FowstID " +
                              " AND inv.DeviceID = devf.DeviceID) " +
                              ",0)"
                End With

                With .Add("SMACAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " +
                              " FROM TransCustomerPaymentInvoices custpayinv " +
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                              " WHERE other.PaymentType = 'SMAC' AND convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                              " AND inv.ID = " + ReadyString(mTransactionID) +
                              " AND inv.FOWSTID = devf.FowstID " +
                              " AND inv.DeviceID = devf.DeviceID) " +
                              ",0)"
                End With

                With .Add("EPlusAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " +
                              " FROM TransCustomerPaymentInvoices custpayinv " +
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                              " WHERE other.PaymentType = 'E-Plus' AND convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                              " AND inv.ID = " + ReadyString(mTransactionID) +
                              " AND inv.FOWSTID = devf.FowstID " +
                              " AND inv.DeviceID = devf.DeviceID) " +
                              ",0)"
                End With

                With .Add("VoucherAmount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT SUM(ISNULL(other.Amount,0)) " +
                              " FROM TransCustomerPaymentInvoices custpayinv " +
                              " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransCustomerPaymentOther other ON (other.CustomerPaymentID = custpay.ID) " +
                              " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                              " WHERE other.PaymentType = 'Discount Voucher / On Line Deals' AND convert(date,custpay.PaymentDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                              " AND inv.ID = " + ReadyString(mTransactionID) +
                              " AND inv.FOWSTID = devf.FowstID " +
                              " AND inv.DeviceID = devf.DeviceID) " +
                              ",0)"
                End With

                With .Add("CurTransCountInv", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT COUNT(inv.InvoiceNumber) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'  " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID)" +
                               ",0)"
                End With

                With .Add("CurTransInvItemCount", SqlDbType.Money)
                    .Expression = "ISNULL((SELECT COUNT(invitem.ProductID) " +
                               " FROM TransCustomerPaymentInvoices custpayinv " +
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " +
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " +
                               " INNER JOIN TransInvoiceItems invitem ON (invitem.InvoiceID = inv.ID) " +
                               " WHERE inv.InvoiceNumber > 0 AND custpay.ReceiptType <> 'RF'  " +
                               " AND convert(date,inv.InvoiceDate) BETWEEN " + ReadyString((mDate)) + " AND " + ReadyString((mDate)) +
                               " AND inv.ID = " + ReadyString(mTransactionID) +
                               " AND inv.FOWSTID = devf.FowstID " +
                               " AND inv.DeviceID = devf.DeviceID" +
                               " AND invitem.Quantity > 0)" & vbCrLf &
                               ",0)"
                End With

                With .Add("CurTotalVatablePortionOfSCDTransaction", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.Vatable) " &
                               " FROM TransCustomerPaymentInvoices custpayinv " &
                               " INNER JOIN TransCustomerPayment custpay ON (custpayinv.CustomerPaymentID = custpay.ID) " &
                               " INNER JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  " &
                               " WHERE inv.InvoiceNumber > 0  AND custpay.ReceiptType <> 'RF' AND inv.ForegoneVAT>0" &
                               " AND inv.ID = " + ReadyString(mTransactionID) &
                               " AND CONVERT(date, inv.InvoiceDate) = " & ReadyString(mDate) & '" AND " & ReadyString(mDate) &
                               " AND inv.FOWSTID = devf.FowstID " &
                               " AND inv.DeviceID = devf.DeviceID) " &
                               ",0)"
                End With

                With .Add("DiscountRefund", SqlDbType.Money)
                    .Expression = " ISNULL((SELECT SUM(inv.SCDiscount) " &
                               " FROM TransSalesReturnLedger as ledger " &
                               " INNER JOIN TransMemo as memo ON memo.ID = ledger.SourceMemoID " &
                               " INNER JOIN TransCustomerPaymentInvoices as cuspayinv ON cuspayinv.InvoiceID = ledger.SourceInvoiceID  " &
                               " INNER JOIN TransCustomerPayment custpay ON cuspayinv.CustomerPaymentID = custpay.ID " &
                               " INNER JOIN TransInvoice inv ON (cuspayinv.InvoiceID = inv.ID)  " &
                               " WHERE ISNULL(RefundNum, 0) > 0 AND CONVERT(date, ledger.LastUpdated) = " & ReadyString(mDate) &
                               " AND inv.ID = " + ReadyString(mTransactionID) &
                               " AND ledger.FOWSTID = devf.FowstID " & vbCrLf &
                               " AND ledger.DeviceID = devf.DeviceID)" & vbCrLf &
                               ",0)"
                End With

                With .Add("CurTermNum", SqlDbType.Money)
                    .Expression = "dev.DeviceNo"
                End With

                With .Add("FowstNo", SqlDbType.Money)
                    .Expression = "fowst.FOWSTNo"
                End With

                With .Add("ZCount", SqlDbType.Money)
                    .Expression = "devf.ZReadCount"
                End With

                pFinder.SqlFrom = " /* vbA7XJ7CTDKZ vbDYAPDHGR64 */ FROM LibraryDeviceFowst devf " +
                " INNER JOIN LibraryDevice dev ON (devf.DeviceID = dev.ID) " +
                " INNER JOIN MobizLibraryOrganizationFOWST fowst ON (devf.FowstID = fowst.ID) "

                pFinder.SqlWhere = " WHERE devf.FowstID = " + ReadyString(mFOWST) + " AND dev.ID = " + ReadyString(mDevice)

            End With

        End With

        Return pFinder.Requery()
    End Function

    Private Function SendEmailReceipt2(vInvoiceID As String, vDtCustomerPayments As DataTable, vDtCustomerPaymentInvoices As DataTable, Optional vDeviceID As String = "") As Boolean

        Dim pPaymentID As String

        vInvoiceID = ReadyGuid(vInvoiceID)
        For Each pDrPaymentInvoice As DataRow In vDtCustomerPaymentInvoices.Rows
            If vInvoiceID = ReadyGuid(pDrPaymentInvoice("InvoiceID")) Then
                pPaymentID = ReadyGuid(pDrPaymentInvoice("CustomerPaymentID"))
                Exit For
            End If
        Next

        If String.IsNullOrWhiteSpace(pPaymentID) = False Then
            Dim pPayments = vDtCustomerPayments.Select("ID = " & ReadyString(pPaymentID))
            If pPayments.Length > 0 Then
                Dim pEmails = pPayments(0)("CustomerEmailAddresses")
                If String.IsNullOrWhiteSpace(pEmails) = False Then
                    SendEmailReceipt(vInvoiceID, pEmails, vDeviceID)
                End If
            End If
        End If

    End Function

    <HttpPost()>
    Public Function SendEmailReceipt(InvoiceID As String, EmailAddresses As String, Optional DeviceID As String = "") As Boolean
        Dim clsSalesPayments As New SalesPayments
        Dim strReturn As String
        Dim dtaTable As DataTable
        Dim intCntr As Integer
        Dim strTemp As String
        Dim strInvoiceNo As String = ""
        Dim strEmail As String()
        Dim strBody As String = ""
        Dim strmReader As System.IO.StreamReader
        Dim varBusinessName As String

        ' field: CustomerEmailAddresses

        If (EmailAddresses.Trim().Length > 0) Then
            strEmail = EmailAddresses.Trim().Split(",")
            intCntr = 0
            While (intCntr < strEmail.Length)

                If (Not IsEmail(strEmail(intCntr).Trim())) Then
                    AppendToDebug("InvalidEmail")
                    Return False
                End If

                intCntr = intCntr + 1
            End While
        End If

        strmReader = New System.IO.StreamReader(Common.ServerPath & "/Views/SalesPayments/EmailSalesReceipt.vbhtml")
        strBody = strmReader.ReadToEnd

        '''Get Payment Info
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentInvoiceHeaderInfo(InvoiceID, mBusiness.ProData)
        If (dtaTable.Rows.Count > 0) Then

            strBody = strBody.Replace("[CustomerNameHere]", "Customer : " & dtaTable.Rows(0).Item("CustomerName").ToString().Trim())
            strBody = strBody.Replace("[CustomerAddressHere]", "Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
            strBody = strBody.Replace("[CustomerTINHere]", "TIN : " & dtaTable.Rows(0).Item("CustomerTIN").ToString().Trim())

            strBody = strBody.Replace("[CustomerBillingAddress]", "Billing Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())
            strBody = strBody.Replace("[CustomerDeliveryAddress]", "Delivery Address : " & dtaTable.Rows(0).Item("OfficeAddress").ToString().Trim())

            strInvoiceNo = Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim()

            strBody = strBody.Replace("[InvoiceNoHere]", "Sales Invoice No. " & Convert.ToInt32(dtaTable.Rows(0).Item("InvoiceNumber")).ToString("000000000000").Trim())
            strBody = strBody.Replace("[InvoiceDateTimeHere]", "Date/Time : " & Convert.ToDateTime(dtaTable.Rows(0).Item("InvoiceDate")).ToString("MMM dd, yyyy HH:mm").Trim())
            strBody = strBody.Replace("[CashierNameHere]", "Cashier : " & dtaTable.Rows(0).Item("CashierName").ToString().Trim())

        End If

        '''Get Header Info
        Dim FowstID As String = CStr2(dtaTable.Rows(0).Item("OrganizationBranchID"))
        Dim POSAccredNo = mBusiness.SystemDefaults.GetSystemDefaultPublic("Mobiz POS Accreditation Number")

        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getReceiptHeaderInfo(FowstID.ToString().Trim(), mBusiness.ProData, POSAccredNo, DeviceID)

        If (dtaTable.Rows.Count > 0) Then
            'additional by janine. to show in email receipt
            If (dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim() <> "") Then
                varBusinessName = dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim()
            Else
                varBusinessName = dtaTable.Rows(0).Item("CorporateName").ToString().Trim()
            End If

            strBody = strBody.Replace("[TradeNameHere]", dtaTable.Rows(0).Item("BusinessTradeName").ToString().Trim())
            strBody = strBody.Replace("[CorporateNameHere]", dtaTable.Rows(0).Item("CorporateName").ToString().Trim())
            strBody = strBody.Replace("[TINHere]", "TIN " & dtaTable.Rows(0).Item("TIN").ToString().Trim())
            strBody = strBody.Replace("[AddressHere]", dtaTable.Rows(0).Item("CompleteAddress").ToString().Trim())
            strBody = strBody.Replace("[TelNoHere]", "Tel. No. " & dtaTable.Rows(0).Item("TelNo").ToString().Trim())

            strBody = strBody.Replace("[POSAccreditationNo]", "POS Accreditation No. " & dtaTable.Rows(0).Item("POSAccreditationNo").ToString().Trim())
            strBody = strBody.Replace("[POSRegistrationNo]", "POS Registration No. " & dtaTable.Rows(0).Item("POSPermitNo").ToString().Trim())
            strBody = strBody.Replace("[POSMachineNo]", "Machine ID No. (MIN) " & dtaTable.Rows(0).Item("MIN").ToString().Trim())

        End If

        '''Get Payment Items
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptBodyInfo(InvoiceID, mBusiness.ProData)
        If (dtaTable.Rows.Count > 0) Then

            strTemp = "<tbody>"
            intCntr = 0
            While (intCntr < dtaTable.Rows.Count)

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 35%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("StockDescription").ToString().Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 10%; height:30px; border: 1px dotted Gray;"">" & Convert.ToInt32(dtaTable.Rows(intCntr).Item("Quantity")).ToString("#,##0").Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 15%; height:30px; border: 1px dotted Gray;"">" & dtaTable.Rows(intCntr).Item("UnitMeasure").ToString().Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("UnitPrice")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:30px; border: 1px dotted Gray;"">" & Convert.ToDouble(dtaTable.Rows(intCntr).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                intCntr = intCntr + 1

            End While

            strTemp = strTemp & "</tbody>"

            strBody = strBody.Replace("[BodyGoesHere]", strTemp)

        End If

        '''Get Payment Amount
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptFooterAmountsInfo(InvoiceID, mBusiness.ProData)
        If (dtaTable.Rows.Count > 0) Then

            '''SKU and Amount due
            strTemp = "<table style=""width:100%"">"
            strTemp = strTemp & "<thead>"
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">SKU Count</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToInt32(dtaTable.Rows(0).Item("SKUCount")).ToString("#,##0").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Sub Total</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("SubTotal")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            ' additional by janine feb 11 2015 
            If (IsSeniorCitizenDiscount(InvoiceID)) Then
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Less: " & dtaTable.Rows(0).Item("TaxPercent") & "% VAT</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("SubTotal")) - CDec2(dtaTable.Rows(0).Item("TotalAmountDue"))).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Total Amount</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TotalAmountDue")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

            End If


            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Discount Amount</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("DiscountAmount")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            ' additional by janine feb 11 2015 
            If (mBusiness.SystemDefaults.IsEnableBillingOption = "1") Then
                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Charges</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("ChargeAmount")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

                strTemp = strTemp & "<tr>"
                strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
                strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Tip</td>"
                strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TipAmount")).ToString("#,##0.00").Trim() & "</td>"
                strTemp = strTemp & "</tr>"

            End If

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">Net Amount Due</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(CDec2(dtaTable.Rows(0).Item("NetAmountDue")) + CDec2(dtaTable.Rows(0).Item("ChargeAmount")) + CDec2(dtaTable.Rows(0).Item("TipAmount"))).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "</thead>"
            strTemp = strTemp & "</table>"

            strTemp = strTemp & "<br />"

            '''VAT Info
            strTemp = strTemp & "<table style=""width:100%"">"
            strTemp = strTemp & "<thead>"
            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT-able Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxSales")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Exempt Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Zero-Rated Sales</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">0.00</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "<tr>"
            strTemp = strTemp & "<td style=""text-align:left; width: 60.1%; height:20px;""></td>"
            strTemp = strTemp & "<td style=""text-align:left; width: 20%; height:20px;"">VAT Amount</td>"
            strTemp = strTemp & "<td style=""text-align:right; width: 20%; height:20px;"">" & Convert.ToDouble(dtaTable.Rows(0).Item("TaxAmount")).ToString("#,##0.00").Trim() & "</td>"
            strTemp = strTemp & "</tr>"

            strTemp = strTemp & "</thead>"
            strTemp = strTemp & "</table>"

            strBody = strBody.Replace("[FooterGoesHere]", strTemp)

        End If

        '''Get Payment Details
        dtaTable = New DataTable()
        dtaTable = clsSalesPayments.getSalesPaymentReceiptPaymentInfo(InvoiceID, mBusiness.ProData)
        If (dtaTable.Rows.Count > 0) Then

            strBody = strBody.Replace("[CASHPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CashPayment")) <= 0, "", "Cash        : " & Convert.ToDouble(dtaTable.Rows(0).Item("CashPayment")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CHECKPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CheckPaymentAmount")) <= 0, "", "Check       : " & Convert.ToDouble(dtaTable.Rows(0).Item("CheckPaymentAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CREDITCARDPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CreditCardAmount")) <= 0, "", "Credit Card : " & Convert.ToDouble(dtaTable.Rows(0).Item("CreditCardAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[DEBITCARDPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("DebitCardAmount")) <= 0, "", "Debit Card : " & Convert.ToDouble(dtaTable.Rows(0).Item("DebitCardAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[SRVPAYMENT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("SRVAmount")) <= 0, "", "SRV         : " & Convert.ToDouble(dtaTable.Rows(0).Item("SRVAmount")).ToString("#,##0.00").Trim()))
            strBody = strBody.Replace("[CHANGEAMOUNT]", IIf(Convert.ToDouble(dtaTable.Rows(0).Item("CustomerChangeCash")) <= 0, "", "Change      : " & Convert.ToDouble(dtaTable.Rows(0).Item("CustomerChangeCash")).ToString("#,##0.00").Trim()))

        End If

        'SendMail("customercare@mobiz-apps.com",
        '         EmailAddresses,
        '         "Mobiz",
        '         varBusinessName & " - Inv. No. " & strInvoiceNo,
        '         strBody,
        '         "smtp.live.com",
        '         587,
        '         "customercare@mobiz-apps.com",
        '         "MobizCustCare2221", , , , , True)

        SendMail("customercare@email.mobiz-apps.com",
            EmailAddresses,
            "Mobiz",
            varBusinessName & " - Inv. No. " & strInvoiceNo,
            strBody,
            "smtp.pepipost.com",
            587,
            "arvinboggs",
            "p$P.p/Kus7v%3<A@~dN^", , , , , True)

        Return True

    End Function

    Function IsSeniorCitizenDiscount(ByVal vInvoiceID As String) As Boolean
        Dim vDiscountType As String

        Dim pSql As String
        Dim pDT As DataTable

        pSql = "SELECT DiscountTypeID FROM TransInvoiceDiscount WHERE InvoiceID = " & ReadyString(vInvoiceID)

        pDT = mBusiness.ProData.Execute(pSql)

        If (pDT.Rows.Count > 0) Then
            vDiscountType = pDT.Rows(0).Item("DiscountTypeID").ToString()

            If (vDiscountType.ToUpper() = "055A0A33-DD98-47EC-ADD3-8B2009F3944A" Or vDiscountType.ToUpper() = "674AD791-188B-4238-B95C-97B5C2B9D544") Then
                Return True
            Else
                Return False
            End If


        Else
            Return False
        End If


    End Function

    Sub UpdateMonitoringTable(DeviceID As String, FowstID As String, InvoiceDate As String, vInputZipFilename As String)
        Dim pSql As String
        Dim ID As String
        Dim AppVersion As String
        Dim LastSync As String
        Dim IsUpdated As String
        Dim pInfo As String

        pInfo = New ProZip(vInputZipFilename).GetString("info.json")

        Dim jsp As New JavaScriptSerializer
        Dim infodictionary As Dictionary(Of String, String) = jsp.Deserialize(Of Dictionary(Of String, String))(pInfo)

        'End If

        pSql = "Select * from MobizLibraryDeviceFOWSTMonitoring " & vbCrLf &
                "WHERE DeviceID = " & ReadyString(DeviceID) & vbCrLf &
                "AND FOWSTID = " & ReadyString(FowstID)

        ID = CStr2(mBusiness.ProData.ExecuteScalar(pSql))
        mBusiness.ProData.ExecuteSQL(pSql)

        Dim pIU As InsertUpdate

        pIU = New InsertUpdate("MobizLibraryDeviceFOWSTMonitoring")

        pIU("OrgID") = ReadyString(CStr2(infodictionary("OrgID")))
        pIU("DeviceID") = ReadyString(DeviceID)
        pIU("UUID") = ReadyString(CStr2(infodictionary("UUID")))
        pIU("FOWSTID") = ReadyString(FowstID)
        pIU("LastDataFlushReceivedInServer") = "GETDATE()"
        pIU("LastDateOfTransactionFlushedFromDevice") = ReadyString(InvoiceDate)

        pIU("APKBuildDate") = ReadyString(CStr2(infodictionary("AppBuildDate")))
        pIU("LastUpdated") = "GETDATE()"

        If infodictionary.ContainsKey("AppVersionName") Then
            pIU("APKVersion") = ReadyString(CStr2(infodictionary("AppVersionName")))
        Else
            pIU("APKVersion") = ReadyString(CStr2(infodictionary("AppVersion")))
        End If

        If infodictionary.ContainsKey("DatabaseCreationStart") Then
            pIU("LastDataSync") = ReadyString(CStr2(infodictionary("DatabaseCreationStart")))
            pIU("InstalledDatabaseCreationDate") = ReadyString(CStr2(infodictionary("DatabaseCreationEnd")))
        End If

        If Not ID = vbNullString Then
            pSql = pIU.GetUpdate("ID = " & ReadyString(ID))
        Else
            pSql = pIU.GetInsert()
        End If

        mBusiness.ProData.ExecuteSQL(pSql)

        pSql = "Select top 1 ID from TransDeviceAvailability WHERE DeviceID = " & ReadyString(DeviceID) & " AND convert(date,TransactionDate) =  " & ReadyString(InvoiceDate)

        IsUpdated = CStr2(mBusiness.ProData.ExecuteScalar(pSql))

        pIU = New InsertUpdate("TransDeviceAvailability")

        pIU("DeviceID") = ReadyString(DeviceID)
        pIU("TransactionDate") = ReadyString(InvoiceDate)
        pIU("IsAvailable") = 1

        If IsUpdated.Length = 0 Then
            pSql = pIU.GetInsert()

            mBusiness.ProData.ExecuteSQL(pSql)
        Else
            pSql = pIU.GetUpdate("ID = " & ReadyString(IsUpdated))

            mBusiness.ProData.ExecuteSQL(pSql)
        End If

        pIU = New InsertUpdate("MobizLibraryOrganizationFOWST")
        pIU("LastSalesDataImportedToServer") = ReadyString(InvoiceDate)
        mBusiness.ProData.Execute(pIU.GetUpdate(" ID = " & ReadyString(FowstID)))

    End Sub

    Private ReadOnly mAddLogLock As New Object
    Private mLastestLogEndTime As Date?

    Private Sub AddLog(vText As String)

        SyncLock mAddLogLock
            If mLogFilename Is Nothing Then
                mLogFilename = Common.ServerPath & "temp\logs\ImportDataClass\" & DateTimeStamp() & "-" & GenerateString(10) & ".txt"
                CreateFolder(mLogFilename)
            End If

            Dim pText As String
            Dim pCurrentTime = Now()
            If mLastestLogEndTime IsNot Nothing Then
                pText &= FormatTime((pCurrentTime - mLastestLogEndTime.Value).TotalSeconds) & " | "
            End If
            pText &= vText
            If mLastestLogEndTime IsNot Nothing Then
                pText &= " | from " & Format(mLastestLogEndTime, cGlobal.FormatDateTimeSql2) & " to " & Format(pCurrentTime, cGlobal.FormatDateTimeSql2) & "`"
            End If
            pText &= vbCrLf
            mLastestLogEndTime = pCurrentTime

            AppendToFile(mLogFilename, pText)
        End SyncLock

    End Sub

    Public Shared Function SummarizeLogs() As String

        Dim pAllLogData = New List(Of LogData)
        Dim pFolderPath As String = Common.ServerPath & "temp\logs\ImportDataClass\"
        For Each pFile In IO.Directory.GetFiles(pFolderPath, "*.txt")
            Dim pFileLogData = ReadLogForSummary(pFile)
            pAllLogData.AddRange(pFileLogData)
        Next

        Dim pOut As New Dictionary(Of String, Integer)
        For Each pLogData In pAllLogData
            If pOut.ContainsKey(pLogData.Task) Then
                pOut(pLogData.Task) += pLogData.Seconds
            Else
                pOut.Add(pLogData.Task, pLogData.Seconds)
            End If
        Next

        Dim pSorted = pOut.OrderByDescending(Function(v As KeyValuePair(Of String, Integer)) v.Value)
        Dim pAllText As String
        For Each pItem In pSorted
            pAllText &= FormatTime(pItem.Value) & " | " & pItem.Key & vbCrLf
        Next

        Return pAllText

    End Function

    Private Class LogData
        Public Task As String
        Public Seconds As Integer
    End Class

    Private Shared Function ReadLogForSummary(vFilename As String) As List(Of LogData)

        Dim pOut = New List(Of LogData)

        Dim pFileText As String
        pFileText = FileText(vFilename)
        Dim pLines = pFileText.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        For Each pLine In pLines
            Dim pIndex = pLine.IndexOf("|")
            If pIndex = -1 Then
                Continue For
            End If
            Dim pFormattedTime = pLine.Substring(0, pIndex)
            Dim pSeconds = ParseTime(pFormattedTime)
            If pSeconds = -1 Then
                Continue For
            End If
            Dim pIndex2 = pLine.IndexOf("|", pIndex + 1)
            Dim pTask = pLine.Substring(pIndex + 1, pIndex2 - pIndex - 1).Trim()
            Dim pLogData = New LogData()
            pLogData.Task = pTask
            pLogData.Seconds = pSeconds
            pOut.Add(pLogData)
        Next

        Return pOut

    End Function

    ''' <summary>
    ''' return the total number of seconds
    ''' </summary>
    ''' <param name="vText"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ParseTime(vText As String) As Integer

        Dim pOut As Integer
        Dim pSplits = vText.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If pSplits.Count <> 3 Then
            Return -1
        End If

        pOut = CInt2(pSplits(0)) * 60 * 60
        pOut += CInt2(pSplits(1)) * 60
        pOut += CInt2(pSplits(2))

        Return pOut

    End Function

    Private Sub AddDummyRecords(vInputZipFilename As String)

        'Dim pNewFilename = AppendBackslash(IO.Path.GetDirectoryName(vInputZipFilename))
        'pNewFilename &= DateTimeStamp() & ".zip"
        'CopyFile(vInputZipFilename, pNewFilename)
        Dim pNewZip = New ProZip(vInputZipFilename)

        Dim pCsvReader = New CsvReader(vInputZipFilename, "TransInvoice")
        Dim pCsvReaderItems = New CsvReader(vInputZipFilename, "TransInvoiceItems")
        Dim pLine = pCsvReader.ReadLine()
        Dim pLineItems = pCsvReaderItems.ReadLine()
        If pLine Is Nothing Then
            Return
        End If
        If pLineItems Is Nothing Then
            Return
        End If
        Dim pDT = New DataTable
        For Each pKeyValue In pLine
            pDT.Columns.Add(pKeyValue.Key)
        Next

        Dim pDtItems = New DataTable
        For Each pKeyValue In pLineItems
            pDtItems.Columns.Add(pKeyValue.Key)
        Next

        For pY = 1 To 100
            Dim pDR = pDT.NewRow()
            Dim pID = ReadyGuid(Guid.NewGuid())
            For Each pKeyValue In pLine
                If pKeyValue.Key = "ID" Then
                    pDR(pKeyValue.Key) = pID
                Else
                    pDR(pKeyValue.Key) = pKeyValue.Value
                End If
            Next
            pDT.Rows.Add(pDR)

            Dim pDrItem = pDtItems.NewRow()
            Dim pIdItem = ReadyGuid(Guid.NewGuid())
            For Each pKeyValue In pLineItems
                If pKeyValue.Key = "ID" Then
                    pDrItem(pKeyValue.Key) = pIdItem
                ElseIf pKeyValue.Key.ToLower() = "InvoiceID".ToLower() Then
                    pDrItem(pKeyValue.Key) = pID
                Else
                    pDrItem(pKeyValue.Key) = pKeyValue.Value
                End If
            Next
            pDtItems.Rows.Add(pDrItem)
        Next
        pNewZip.AddTableCsv(pDT, "TransInvoice")
        pNewZip.AddTableCsv(pDtItems, "TransInvoiceItems")

    End Sub

    Public Class FlushingSettingsClass
        Public TimeFrom As String
        Public TimeTo As String
    End Class

    Shared mFlushTimeFrom As DateTime?
    Shared mFlushTimeTo As DateTime?
    Shared mHasFlushSettings As Boolean?

    Public Shared Function OpenSettings(ByRef oResult As String) As Boolean

        Try
            Dim pFilename = IO.Path.Combine(cGlobal.AppPath, "settings", "Flushing.txt")
            If IO.File.Exists(pFilename) = False Then
                oResult = "File, " & pFilename & ", does not exist. vbZG84LQ87BC!"
                Return False
            End If
            Dim pFileText = FileText(pFilename)
            If String.IsNullOrWhiteSpace(pFileText) Then
                oResult = "File, " & pFilename & ", is blank. vbU5SNAU7BVG!"
                Return False
            End If
            Dim pJson = JsonConvert.DeserializeObject(Of FlushingSettingsClass)(pFileText)
            mFlushTimeFrom = ParseDate(pJson.TimeFrom)
            If mFlushTimeFrom.HasValue = False Then
                oResult = "TimeFrom must be a valid time. " & pJson.TimeFrom
                Return False
            End If
            mFlushTimeTo = ParseDate(pJson.TimeTo)
            If mFlushTimeTo.HasValue = False Then
                oResult = "TimeTo must be a valid time. " & pJson.TimeFrom
                Return False
            End If

            oResult = "TimeFrom: " & mFlushTimeFrom.Value.ToString("h:mm tt") & vbCrLf &
                "TimeTo: " & mFlushTimeTo.Value.ToString("h:mm tt")
            Return True
        Catch ex As Exception
            Return ex.Message & vbCrLf &
                GetStackTrace(, ex)
        End Try

    End Function

    Public Shared Function CanFlushNow(Optional vNow As DateTime? = Nothing, Optional ByRef oMessage As String = Nothing) As Boolean

        If mHasFlushSettings Is Nothing Then
            mHasFlushSettings = OpenSettings(Nothing)
            Return CanFlushNow(vNow, oMessage)
        End If
        If mHasFlushSettings = False Then
            Return True
        End If

        If vNow Is Nothing Then
            vNow = Now
        End If

        mFlushTimeFrom = New DateTime(vNow.Value.Year, vNow.Value.Month, vNow.Value.Day, mFlushTimeFrom.Value.Hour, mFlushTimeFrom.Value.Minute, 0)
        mFlushTimeTo = New DateTime(vNow.Value.Year, vNow.Value.Month, vNow.Value.Day, mFlushTimeTo.Value.Hour, mFlushTimeTo.Value.Minute, 0)
        If mFlushTimeFrom.Value.TimeOfDay > mFlushTimeTo.Value.TimeOfDay Then
            If mFlushTimeFrom.Value < vNow.Value Then
                mFlushTimeTo = mFlushTimeTo.Value.AddDays(1)
            Else
                mFlushTimeFrom = mFlushTimeFrom.Value.AddDays(-1)
            End If
        End If

        Dim pOut = mFlushTimeFrom.Value <= vNow _
            AndAlso vNow <= mFlushTimeTo.Value
        oMessage = "TimeFrom: " & mFlushTimeFrom.Value.ToString(cGlobal.FormatDateTime) & vbCrLf &
            "TimeTo: " & mFlushTimeTo.Value.ToString(cGlobal.FormatDateTime) & vbCrLf &
            "Current time: " & vNow.Value.ToString(cGlobal.FormatDateTime) & vbCrLf

        Return pOut

    End Function

    Public Shared Function CreateSettings(ByRef oResult As String) As Boolean

        If OpenSettings(Nothing) Then
            oResult = "Settings is already set. vbVHN965JCL2!"
            Return False
        End If

        Dim pFilename = IO.Path.Combine(cGlobal.AppPath, "settings", "Flushing.txt")
        Dim pJson = New FlushingSettingsClass()
        pJson.TimeFrom = Format(Now.Date, "h:mm tt")
        pJson.TimeTo = Format(Now.Date.AddDays(1).AddSeconds(-1), "h:mm tt")
        Dim pFileText = JsonConvert.SerializeObject(pJson, Formatting.Indented)
        FileText(pFilename) = pFileText

        Return True

    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Try
                    If mBusiness Is Nothing _
                        AndAlso mBusiness.ProData IsNot Nothing Then

                        mBusiness.ProData.Dispose()
                    End If
                Catch ex As Exception
                    Debug.WriteLine("vbDQPCAD3WX7! just ignore these errors in ImportDataClass.Dispose: " & ex.Message)
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

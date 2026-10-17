Imports System.Threading
Imports Aspose.Cells
Imports Newtonsoft.Json

Namespace WebProj.Data
    Public Class FranchiseeDataExporter
        ReadOnly mProData As ProData
        'Public FranchiseeOrgFowstID As String ' source database
        Public StartDate As Date = Now.AddMonths(-1) '#9/18/2014#
        Public EndDate As Date = Now '#9/18/2014#
        ReadOnly mBusiness As Business
        Shared ReadOnly mSemaphore As New SemaphoreSlim(1, 1)
        Const mTop As Integer = 250
        Dim mHasIgnoreError As Boolean
        Public StartFresh As Boolean
        Dim mFolder As String
        Dim mDtFowsts As DataTable
        Public DontStart As Boolean
        Public DetectInvoicesWithNoItems As Boolean
        Public FowstID As String ' optional filter
        Dim mFowstIndex As Integer
        Dim mLastMessage As String
        Dim mLastMessageDate As Date

        ''' <summary>
        ''' optional. if present, use this as the source of data
        ''' </summary>
        Public ForceFranchiseeDatabase As String

        ''' <summary>
        ''' optional. if present, use this as the destination of data
        ''' </summary>
        Public ForceFranchisorDatabase As String

        ' these variables are for viewing progress only
        Public Shared DateStarted As Date
        Public Shared CurrentFranchiseeOrgFowstID As String
        Public Shared ErrorMessage As String
        Public Shared RecordsSaved As Integer
        Public Shared ProgressInfo As Info
        Public Shared Instance As FranchiseeDataExporter

        Public Sub New(vProData As ProData)

            mProData = vProData ' ProData.CreateFromConnectionInfo()
            mBusiness = New Business(mProData)

        End Sub

        ReadOnly Property ProData As ProData
            Get

                Return mProData

            End Get
        End Property

        Public Shared Function IsBusy() As Boolean

            If mSemaphore.Wait(2000) = False Then
                Return True
            End If
            mSemaphore.Release()

        End Function

        Public Function CopyRecords(vFranchiseeOrgID As String) As Integer

            If String.IsNullOrWhiteSpace(ForceFranchiseeDatabase) = False _
                AndAlso String.IsNullOrWhiteSpace(ForceFranchisorDatabase) Then

                Throw New ApplicationException("vbN3KKB6KT7W! ForceFranchiseeDatabase is set. ForceFranchisorDatabase must also be set.")
            End If
            If String.IsNullOrWhiteSpace(ForceFranchisorDatabase) = False _
                AndAlso String.IsNullOrWhiteSpace(ForceFranchiseeDatabase) Then

                Throw New ApplicationException("vb2WMZMQK9DR! ForceFranchisorDatabase is set. ForceFranchiseeDatabase must also be set.")
            End If
            If String.IsNullOrWhiteSpace(ForceFranchisorDatabase) = False _
                AndAlso String.IsNullOrWhiteSpace(ForceFranchiseeDatabase) = False Then

                If ForceFranchiseeDatabase.Trim.ToLower() = ForceFranchisorDatabase.Trim().ToLower() Then
                    Throw New ApplicationException("vb497FLYLLTQ! ForceFranchiseeDatabase and ForceFranchisorDatabase must not be equal.")
                End If
            End If

            If mSemaphore.Wait(2000) = False Then
                Throw New ApplicationException("Another CopyRecords process is in progress. Time started: " & Format(DateStarted, cGlobal.FormatDateTimeSql) & ". vbQ2R3Z5LAH4!")
            End If
            Instance = Me

            Try
                If DontStart Then
                    Return -1
                End If

                If StartFresh Then
                    mFolder = Common.ServerPath & "temp\FranchiseeDataExporter\" & DateTimeStamp() & "\"
                Else
                    mFolder = Common.ServerPath & "temp\FranchiseeDataExporter\"
                    If IO.Directory.Exists(mFolder) Then
                        Dim pDirectories = IO.Directory.GetDirectories(mFolder)
                        Dim pSorted =
                        From p In pDirectories
                        Select p
                        Order By p Descending

                        For Each pDirectory In pSorted
                            If IO.File.Exists(IO.Path.Combine(pDirectory, "Done.txt")) Then
                                Exit For
                            End If
                            Dim pInfoText = FileText(IO.Path.Combine(pDirectory, "info.txt"))
                            If pInfoText = "" Then
                                Exit For
                            End If
                            ProgressInfo = JsonConvert.DeserializeObject(Of Info)(pInfoText)
                            Dim pExcelFilename = IO.Path.Combine(pDirectory, "fowsts.xlsx")
                            If IO.File.Exists(pExcelFilename) Then
                                Dim pWB = New Workbook()
                                Try
                                    pWB.Open(pExcelFilename)
                                Catch ex As Exception
                                    If ex.Message.ToLower().Contains("Cannot find central directory".ToLower()) Then
                                        Continue For
                                    End If
                                    Throw
                                End Try
                                Dim pExcelHeaders = GetExcelHeaders(pWB.Worksheets(0))
                                Dim pError As String
                                mDtFowsts = ExcelToTable(pExcelFilename, pExcelHeaders.ToArray(), pError)
                                mFolder = AppendBackslash(pDirectory)
                            End If
                            Exit For
                        Next
                    End If
                End If
                If mDtFowsts IsNot Nothing Then
                    mDtFowsts.DefaultView.RowFilter = "IsValid = true" ' dont filter by TimeDone = '' 
                    'If mDtFowsts.DefaultView.Count = 0 Then
                    '    FileText(mFolder & "Done.txt") = Now ' for backward compatibility with existing folders
                    '    mDtFowsts = Nothing
                    'End If
                End If

                If mDtFowsts Is Nothing Then
                    mFolder = Common.ServerPath & "temp\FranchiseeDataExporter\" & DateTimeStamp() & "\"

                    Try
                        CreateFolder(mFolder & "fowsts.xlsx")
                        mDtFowsts = mBusiness.LibraryOrganization.GetFranchisors(vFranchiseeOrgID, FowstID)

                        If String.IsNullOrWhiteSpace(ForceFranchiseeDatabase) = False Then
                            Dim pDtTransactionFowsts = GetFowstsWithTransactions()
                            MergeFowstTables(mDtFowsts, pDtTransactionFowsts)
                        End If

                        mDtFowsts.Columns.Add("TimeDone", GetType(DateTime))
                        TableToExcelFile(mDtFowsts, "", "", mFolder & "fowsts.xlsx")
                    Catch ex As Exception
                        AppendToLog("vbMB67SZ8U5B! " & ex.Message, mFolder)
                    End Try
                    mDtFowsts.DefaultView.RowFilter = "IsValid = true"
                    ProgressInfo = New Info
                    ProgressInfo.StartDate = StartDate
                    ProgressInfo.EndDate = EndDate
                    ProgressInfo.DetectInvoicesWithNoItems = DetectInvoicesWithNoItems
                    ProgressInfo.FowstID = FowstID
                    Dim pInfoText = JsonConvert.SerializeObject(ProgressInfo, Formatting.Indented)
                    FileText(mFolder & "info.txt") = pInfoText
                End If

                DateStarted = Now

                RecordsSaved = 0
                Try
                    AppendToLog("vbP7JXFD6YR4! CopyRecords start" & vbCrLf &
                    "vFranchiseeOrgID = " & vFranchiseeOrgID & vbCrLf &
                    "FranchiseeOrgFowstID = " & FowstID & vbCrLf &
                    "TransStartDate = " & StartDate.ToString(cGlobal.FormatDateTime) & vbCrLf &
                    "TransEndDate = " & EndDate.ToString(cGlobal.FormatDateTime) & vbCrLf &
                    "DetectInvoicesWithNoItems = " & DetectInvoicesWithNoItems & vbCrLf &
                    "StartFresh = " & StartFresh, mFolder)

                    RecordsSaved = 0
                    Dim pErrorCount = 0

                    Do
                        StartDate = ProgressInfo.StartDate.AddDays(ProgressInfo.DateOffset)
                        If StartDate > ProgressInfo.EndDate Then
                            Exit Do
                        End If
                        EndDate = StartDate
                        mFowstIndex = 0

                        For Each pDRV As DataRowView In mDtFowsts.DefaultView
                            If CStr2(pDRV("TimeDone")) <> "" Then
                                Continue For
                            End If
                            'AppendToLog("vbZ56YYWGX4W! CopyRecords loop" & vbCrLf &
                            '    "FranchiseeOrgFowstID=" & CStr2(pDRV("FranchiseeOrgFowstID")), , "FranchiseeDataExporter")
                            CurrentFranchiseeOrgFowstID = ReadyGuid(pDRV("FranchiseeOrgFowstID"))
                            'If Now.Date = #7/25/2023# Then
                            '    If CurrentFranchiseeOrgFowstID <> ReadyGuid("6ba0ff51-2e1c-4e3f-bc72-b8332f4464b1") Then
                            '        Continue For
                            '    End If
                            '    AppendToLog("vbXBW9N6S65C! CurrentFranchiseeOrgFowstID=" & CurrentFranchiseeOrgFowstID & vbCrLf &
                            '        "vFranchiseeOrgID=" & vFranchiseeOrgID, , "FranchiseeDataExporter")
                            'End If

                            RecordsSaved += StartCopy(
                            vFranchiseeOrgID:=CStr2(pDRV("FranchiseeOrgID")),
                            vFranchiseeFowstID:=CStr2(pDRV("FranchiseeOrgFowstID")),
                            vFranchiseeDatabase:=CStr2(pDRV("FranchiseeDatabase")),
                            vFranchisorOrgID:=CStr2(pDRV("FranchisorOrgID")),
                            vFranchisorDatabase:=CStr2(pDRV("FranchisorDatabase")),
                            vFranchiseGroupID:=CStr2(pDRV("FranchiseGroupID")),
                            vFranchisorGroupName:=CStr2(pDRV("FranchisorGroupName")),
                            vRegionalGroupID:=CStr2(pDRV("RegionalGroupID")),
                            vRegionName:=CStr2(pDRV("RegionName")),
                            vFranchisorFowstID:=CStr2(pDRV("FranchisorFowstID")))

                            'pDtCopied = CopyCacheInventoryMovement(
                            '    vFranchiseeOrgID:=CStr2(pDRV("FranchiseeOrgID")),
                            '    vFranchiseeFowstID:=CStr2(pDRV("FranchiseeOrgFowstID")),
                            '    vFranchiseeDatabase:=pDRV("FranchiseeDatabase"),
                            '    vFranchisorOrgID:=CStr2(pDRV("FranchisorOrgID")),
                            '    vFranchisorDatabase:=pDRV("FranchisorDatabase"),
                            '    vFranchisorFowstID:=pDRV("FranchisorFowstID"))
                            'pTotalRecords += pDtCopied.Rows.Count
                            'RecordsSaved += pDtCopied.Rows.Count
                            'pDtCopied = CopyCacheInvoices(
                            '    vFranchiseeOrgID:=CStr2(pDRV("FranchiseeOrgID")),
                            '    vFranchiseeFowstID:=CStr2(pDRV("FranchiseeOrgFowstID")),
                            '    vFranchiseeDatabase:=pDRV("FranchiseeDatabase"),
                            '    vFranchisorOrgID:=CStr2(pDRV("FranchisorOrgID")),
                            '    vFranchisorDatabase:=pDRV("FranchisorDatabase"),
                            '    vFranchisorFowstID:=pDRV("FranchisorFowstID"),
                            '    vFranchiseGroupID:=CStr2(pDRV("FranchiseGroupID")),
                            '    vFranchisorGroupName:=CStr2(pDRV("FranchisorGroupName")),
                            '    vRegionalGroupID:=CStr2(pDRV("RegionalGroupID")),
                            '    vRegionName:=CStr2(pDRV("RegionName")))
                            'pTotalRecords += pDtCopied.Rows.Count
                            'RecordsSaved += pDtCopied.Rows.Count
                            'pDtCopied = CopySalesReturns(
                            '    vFranchiseeOrgID:=CStr2(pDRV("FranchiseeOrgID")),
                            '    vFranchiseeFowstID:=CStr2(pDRV("FranchiseeOrgFowstID")),
                            '    vFranchiseeDatabase:=pDRV("FranchiseeDatabase"),
                            '    vFranchisorOrgID:=CStr2(pDRV("FranchisorOrgID")),
                            '    vFranchisorDatabase:=pDRV("FranchisorDatabase"),
                            '    vFranchisorFowstID:=pDRV("FranchisorFowstID"))
                            'pTotalRecords += pDtCopied.Rows.Count
                            'RecordsSaved += pDtCopied.Rows.Count
                            pDRV("TimeDone") = Now
                            TableToExcelFile(mDtFowsts, "", "", mFolder & "fowsts.xlsx")
                            AppendToLog("vbZZ5GHU7QYC! CurrentFranchiseeOrgFowstID=" & CurrentFranchiseeOrgFowstID & vbCrLf &
                            "Franchisee " & (mFowstIndex + 1) & " of " & mDtFowsts.DefaultView.Count & vbCrLf &
                            "vFranchiseeOrgID=" & vFranchiseeOrgID & vbCrLf &
                            "StartDate = " & StartDate.ToString(cGlobal.FormatDate) & vbCrLf &
                            "EndDate = " & EndDate.ToString(cGlobal.FormatDate) & vbCrLf, mFolder)
                            mFowstIndex += 1
                        Next pDRV
                        For Each pDRV As DataRowView In mDtFowsts.DefaultView
                            pDRV("TimeDone") = DBNull.Value
                        Next
                        TableToExcelFile(mDtFowsts, "", "", mFolder & "fowsts.xlsx")
                        ProgressInfo.DateOffset += 1
                        FileText(mFolder & "info.txt") = JsonConvert.SerializeObject(ProgressInfo, Formatting.Indented)
                    Loop

                    RetryFailed()

                    If ProgressInfo.Errors?.Count = 0 Then
                        FileText(mFolder & "Done.txt") = Now
                    End If
                    AppendToLog("vbYFYST44Z4B! CopyRecords end" & vbCrLf &
                    "vFranchiseeOrgID=" & vFranchiseeOrgID, mFolder)
#If DEBUG Then
                    Thread.Sleep(5000)
#End If
                Catch ex As Exception
                    AppendToDebug("vbKVMWRJH3YM! CopyRecords error" & vbCrLf &
                    "vFranchiseeOrgID=" & vFranchiseeOrgID,
                    True, ex, True)
                End Try
                Return RecordsSaved
            Catch ex As Exception
                AppendToLog("vbCYGPEHEN7Z! " & ex.Message & vbCrLf &
                GetStackTrace(, ex), mFolder)
            Finally
                mSemaphore.Release()
            End Try

        End Function

        Public ReadOnly Property FowstCount() As Integer
            Get
                Return mDtFowsts?.DefaultView.Count
            End Get
        End Property

        Public ReadOnly Property FowstIndex As Integer
            Get
                Return mFowstIndex
            End Get
        End Property

        Public Property LastMessage As String
            Get
                Return mLastMessage
            End Get
            Set(value As String)
                mLastMessage = value
                mLastMessageDate = Now
            End Set
        End Property

        Public ReadOnly Property LastMessageDate As Date
            Get
                Return mLastMessageDate
            End Get
        End Property

        Function GetFowstsWithTransactions() As DataTable

            Dim pSql = "SELECT DISTINCT FowstID" & vbCrLf &
                " FROM " & EscapeSqlName(ForceFranchiseeDatabase) & ".dbo.CacheInvoices" & vbCrLf &
                " WHERE CONVERT(date, CacheInvoices.InvoiceDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
                " ORDER BY FowstID"
            Dim pDT = mProData.Execute(pSql)
            Return pDT

        End Function

        Sub MergeFowstTables(ByRef rDtBase As DataTable, vDtAddables As DataTable)

            For Each pDrAddable As DataRow In vDtAddables.Rows
                Dim pSelectedFowsts = rDtBase.Select("FranchiseeOrgFowstID = " & ReadyString(pDrAddable("FowstID")))
                If pSelectedFowsts.Any() Then
                    For Each pDrSelectedFowst In pSelectedFowsts
                        pDrSelectedFowst("IsValid") = True
                    Next
                Else
                    Dim pDrNew = rDtBase.NewRow
                    pDrNew("FranchiseeOrgFowstID") = pDrAddable("FowstID")
                    pDrNew("IsValid") = True
                    rDtBase.Rows.Add(pDrNew)
                End If
            Next

            For Each pDR In rDtBase.Rows
                pDR("FranchiseeDatabase") = ForceFranchiseeDatabase
                pDR("FranchisorDatabase") = ForceFranchisorDatabase
            Next

        End Sub

        ''' <summary>
        ''' return the total number of records copied
        ''' </summary>
        ''' <param name="vFranchiseeOrgID"></param>
        ''' <param name="vFranchiseeDatabase"></param>
        ''' <param name="vFranchiseeFowstID"></param>
        ''' <param name="vFranchisorOrgID"></param>
        ''' <param name="vFranchisorDatabase"></param>
        ''' <param name="vFranchisorFowstID"></param>
        ''' <returns></returns>
        Function StartCopy(ByVal vFranchiseeOrgID As String,
            ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
            vFranchisorOrgID As String,
            ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String,
            vFranchiseGroupID As String, vFranchisorGroupName As String,
            vRegionalGroupID As String, vRegionName As String) As Integer

            Dim pTotalRecords As Integer

            Try
                '// commented 2024_07_23__13_32_32_3783
                'pTotalRecords += CopyInvoices(
                '    vFranchiseeOrgID:=vFranchiseeOrgID,
                '    vFranchiseeFowstID:=vFranchiseeFowstID,
                '    vFranchiseeDatabase:=vFranchiseeDatabase,
                '    vFranchisorOrgID:=vFranchisorOrgID,
                '    vFranchisorDatabase:=vFranchisorDatabase,
                '    vFranchisorFowstID:=vFranchisorFowstID)

                'pDtCopied = CopyCacheInventoryMovement(
                '    vFranchiseeOrgID:=CStr2(pDRV("FranchiseeOrgID")),
                '    vFranchiseeFowstID:=CStr2(pDRV("FranchiseeOrgFowstID")),
                '    vFranchiseeDatabase:=pDRV("FranchiseeDatabase"),
                '    vFranchisorOrgID:=CStr2(pDRV("FranchisorOrgID")),
                '    vFranchisorDatabase:=pDRV("FranchisorDatabase"),
                '    vFranchisorFowstID:=pDRV("FranchisorFowstID"))
                'pTotalRecords += pDtCopied.Rows.Count
                'RecordsSaved += pDtCopied.Rows.Count

                '#If DEBUG = False Then
                ' commented 2025_08_08__07_49_19_7632
                'pTotalRecords += CopyCacheInvoices(
                'vFranchiseeOrgID:=vFranchiseeOrgID,
                'vFranchiseeFowstID:=vFranchiseeFowstID,
                'vFranchiseeDatabase:=vFranchiseeDatabase,
                'vFranchisorOrgID:=vFranchisorOrgID,
                'vFranchisorDatabase:=vFranchisorDatabase,
                'vFranchisorFowstID:=vFranchisorFowstID,
                'vFranchiseGroupID:=vFranchiseGroupID,
                'vFranchisorGroupName:=vFranchisorGroupName,
                'vRegionalGroupID:=vRegionalGroupID,
                'vRegionName:=vRegionName)

                pTotalRecords += CopyCacheInvoiceDaily(
                vFranchiseeOrgID:=vFranchiseeOrgID,
                vFranchiseeFowstID:=vFranchiseeFowstID,
                vFranchiseeDatabase:=vFranchiseeDatabase,
                vFranchisorOrgID:=vFranchisorOrgID,
                vFranchisorDatabase:=vFranchisorDatabase,
                vFranchisorFowstID:=vFranchisorFowstID,
                vFranchiseGroupID:=vFranchiseGroupID,
                vFranchisorGroupName:=vFranchisorGroupName,
                vRegionalGroupID:=vRegionalGroupID,
                vRegionName:=vRegionName)

#If DEBUG = False Then
                ' commented 2025_08_13__14_44_22_4138
                'pTotalRecords += CopyCacheInvoiceFinishedProducts(
                'vFranchiseeOrgID:=vFranchiseeOrgID,
                'vFranchiseeFowstID:=vFranchiseeFowstID,
                'vFranchiseeDatabase:=vFranchiseeDatabase,
                'vFranchisorOrgID:=vFranchisorOrgID,
                'vFranchisorDatabase:=vFranchisorDatabase,
                'vFranchisorFowstID:=vFranchisorFowstID,
                'vFranchiseGroupID:=vFranchiseGroupID,
                'vFranchisorGroupName:=vFranchisorGroupName,
                'vRegionalGroupID:=vRegionalGroupID,
                'vRegionName:=vRegionName)
                ''#End If

                'pTotalRecords += CopyCacheInvoiceFlavors(
                'vFranchiseeOrgID:=vFranchiseeOrgID,
                'vFranchiseeFowstID:=vFranchiseeFowstID,
                'vFranchiseeDatabase:=vFranchiseeDatabase,
                'vFranchisorOrgID:=vFranchisorOrgID,
                'vFranchisorDatabase:=vFranchisorDatabase,
                'vFranchisorFowstID:=vFranchisorFowstID,
                'vFranchiseGroupID:=vFranchiseGroupID,
                'vFranchisorGroupName:=vFranchisorGroupName,
                'vRegionalGroupID:=vRegionalGroupID,
                'vRegionName:=vRegionName)
#End If

                'pTotalRecords += pDtCopied.Rows.Count
                'RecordsSaved += pDtCopied.Rows.Count
                'pDtCopied = CopySalesReturns(
                '    vFranchiseeOrgID:=CStr2(pDRV("FranchiseeOrgID")),
                '    vFranchiseeFowstID:=CStr2(pDRV("FranchiseeOrgFowstID")),
                '    vFranchiseeDatabase:=pDRV("FranchiseeDatabase"),
                '    vFranchisorOrgID:=CStr2(pDRV("FranchisorOrgID")),
                '    vFranchisorDatabase:=pDRV("FranchisorDatabase"),
                '    vFranchisorFowstID:=pDRV("FranchisorFowstID"))
                'pTotalRecords += pDtCopied.Rows.Count
                'RecordsSaved += pDtCopied.Rows.Count
            Catch ex As ProSqlException
                Dim pLogReference = GenerateString(10)
                AppendToLog("vbMAXG2JKH8W! vbEATJ23VH73! CopyRecords loop error" & vbCrLf &
                "vFranchiseeOrgID = " & vFranchiseeOrgID & vbCrLf &
                "FranchiseeOrgFowstID = " & vFranchiseeFowstID & vbCrLf &
                "LogReference = " & pLogReference & vbCrLf &
                "pTotalRecords=" & pTotalRecords & vbCrLf &
                ex.Message & vbCrLf & GetStackTrace(, ex) & vbCrLf &
                "SQL: " & ex.Sql, mFolder)
                ProgressInfo.Errors.Add(New Info.ErrorClass() With {
                .ErrorMessage = ex.Message,
                .LogReference = pLogReference,
                .FowstID = vFranchiseeFowstID,
                .DateOffset = ProgressInfo.DateOffset
            })
                FileText(mFolder & "info.txt") = JsonConvert.SerializeObject(ProgressInfo, Formatting.Indented)
            Catch ex As Exception
                Dim pLogReference = GenerateString(10)
                AppendToLog("vbMAXG2JKH8W! CopyRecords loop error" & vbCrLf &
                "vFranchiseeOrgID = " & vFranchiseeOrgID & vbCrLf &
                "FranchiseeOrgFowstID = " & vFranchiseeFowstID & vbCrLf &
                "LogReference = " & pLogReference & vbCrLf &
                "pTotalRecords=" & pTotalRecords & vbCrLf &
                ex.Message & vbCrLf & GetStackTrace(, ex), mFolder)
                ProgressInfo.Errors.Add(New Info.ErrorClass() With {
                .ErrorMessage = ex.Message,
                .LogReference = pLogReference,
                .FowstID = vFranchiseeFowstID,
                .DateOffset = ProgressInfo.DateOffset
            })
                FileText(mFolder & "info.txt") = JsonConvert.SerializeObject(ProgressInfo, Formatting.Indented)
            End Try

            Return pTotalRecords

        End Function

        Function RetryFailed()

            If ProgressInfo.Errors?.Count = 0 Then
                Exit Function
            End If

            AppendToLog("vbTB9HK65N9Y! entering RetryFailed. ProgressInfo.Errors.Count = " & ProgressInfo.Errors.Count, mFolder)
            Do
                Dim pErrorInfo = ProgressInfo.Errors.FirstOrDefault
                If pErrorInfo Is Nothing Then
                    Exit Do
                End If
                Dim pDRs = mDtFowsts.Select("FranchiseeOrgFowstID = " & ReadyString(pErrorInfo.FowstID))
                If pDRs.Length = 0 Then
                    AppendToLog("vbGGNRT8QTC5! RetryFailed FranchiseeOrgFowstID, " & pErrorInfo.FowstID & ", does not exist in the excel file.",
                    mFolder, "CantRetry")
                Else
                    AppendToLog("vbAMGWUNXV6T! RetryFailed. ProgressInfo.Errors.Count = " & ProgressInfo.Errors.Count, mFolder)
                    StartDate = ProgressInfo.StartDate.AddDays(pErrorInfo.DateOffset)
                    EndDate = StartDate
                    RecordsSaved += StartCopy(
                    vFranchiseeOrgID:=CStr2(pDRs(0)("FranchiseeOrgID")),
                    vFranchiseeFowstID:=CStr2(pDRs(0)("FranchiseeOrgFowstID")),
                    vFranchiseeDatabase:=pDRs(0)("FranchiseeDatabase"),
                    vFranchisorOrgID:=CStr2(pDRs(0)("FranchisorOrgID")),
                    vFranchisorDatabase:=pDRs(0)("FranchisorDatabase"),
                    vFranchisorFowstID:=pDRs(0)("FranchisorFowstID"),
                    vFranchiseGroupID:=CStr2(pDRs(0)("FranchiseGroupID")),
                    vFranchisorGroupName:=CStr2(pDRs(0)("FranchisorGroupName")),
                    vRegionalGroupID:=CStr2(pDRs(0)("RegionalGroupID")),
                    vRegionName:=CStr2(pDRs(0)("RegionName")))
                End If
                ProgressInfo.Errors.Remove(pErrorInfo)
                FileText(mFolder & "info.txt") = JsonConvert.SerializeObject(ProgressInfo, Formatting.Indented)
            Loop

        End Function

        Private Function CopyCacheInventoryMovement(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String) As DataTable

            Dim pDT As DataTable
            Dim pSW = Stopwatch.StartNew()

            pDT = GetCopyableCacheInventoryMovement(
            vFranchiseeDatabase:=vFranchiseeDatabase,
            vFranchiseeFowstID:=vFranchiseeFowstID,
            vFranchisorDatabase:=vFranchisorDatabase)
            AppendToLog("vbXCM5NL4VPN! GetCopyableCacheInventoryMovement " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
            "Rows = " & pDT.Rows.Count, mFolder)

            pSW.Restart()
            For Each pDR As DataRow In pDT.Rows
                Dim pIU = Business.DataRowToInsertUpdate(pDR)
                pIU.TableName = EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInventoryMovement"
                Dim pSql = pIU.GetUpdate("ProductID = " & ReadyString(pDR("ProductID")) & vbCrLf &
                " AND FowstID = " & ReadyString(pDR("FowstID")) & vbCrLf &
                " AND TransactionDate = " & ReadyString(Format(pDR("TransactionDate"), cGlobal.FormatDateTimeSql)))
                Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
                If pAffectedRecords = 0 Then
                    pSql = pIU.GetInsert()
                    mProData.ExecuteSQL(pSql)
                End If
            Next
            AppendToLog("vbQL5M5KQXM9! CopyCacheInventoryMovement " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
            "Rows = " & pDT.Rows.Count, mFolder)

            Return pDT

        End Function

        Private Function CopyInvoices(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String) As Integer

            Dim pDtInvoices As DataTable
            Dim pDtItems As DataTable
            Dim pIsPo As Boolean
            Dim pSW = Stopwatch.StartNew()
            Dim pOut As Integer

            Do
                pDtInvoices = GetCopyableInvoices(
                vFranchiseeDatabase:=vFranchiseeDatabase,
                vFranchiseeFowstID:=vFranchiseeFowstID,
                vFranchisorDatabase:=vFranchisorDatabase)
                AppendToLog("vb8XN72P87LP! GetCopyableInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
            "Rows = " & pDtInvoices.Rows.Count, mFolder)
                pOut += pDtInvoices.Rows.Count

                pSW.Restart()
                For Each pDrInvoice As DataRow In pDtInvoices.Rows
                    pIsPo = False
                    If pDrInvoice("PurchaseOrderNo") IsNot DBNull.Value Then
                        If pDrInvoice("SupplierID") Is DBNull.Value Then
                            Continue For
                        End If
                        If ReadyGuid(vFranchisorOrgID) <> ReadyGuid(pDrInvoice("SupplierID")) Then
                            Continue For
                        End If
                        pIsPo = True
                    End If

                    pDtItems = GetInvoiceItems(CStr2(pDrInvoice("ID")), vFranchiseeDatabase)
                    Dim pDtModifiers = GetInvoiceModifiers(CStr2(pDrInvoice("ID")), vFranchiseeDatabase)
                    CopyInvoice(
                    vDR:=pDrInvoice,
                    vDtItems:=pDtItems,
                    vDtModifiers:=pDtModifiers,
                    vFranchiseeOrgID:=vFranchiseeOrgID,
                    vFranchiseeFowstID:=vFranchiseeFowstID,
                    vFranchisorDatabase:=vFranchisorDatabase,
                    vFranchisorFowstID:=vFranchisorFowstID,
                    vIsPo:=pIsPo)
                Next pDrInvoice
                AppendToLog("vbLJ4NXCJ8YG! CopyInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDtInvoices.Rows.Count, mFolder)
                If pDtInvoices.Rows.Count >= mTop Then
                    AppendToLog("vb7FKLYZESU8! repeating copy - CopyInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDtInvoices.Rows.Count, mFolder)
                Else
                    Exit Do
                End If
            Loop

            Return pOut

        End Function

        Private Function GetCopyableInvoices(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
        vFranchisorDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT DISTINCT TOP " & mTop & " tblInvoiceFranchisee.*" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransInvoice tblInvoiceFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoice tblInvoiceFranchisor ON tblInvoiceFranchisor.ID = tblInvoiceFranchisee.ID" & vbCrLf
            If DetectInvoicesWithNoItems Then
                pSql &= " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoiceItems tblInvoiceItemsFranchisor ON tblInvoiceItemsFranchisor.InvoiceID = tblInvoiceFranchisee.ID" & vbCrLf
                pSql &= " WHERE (tblInvoiceFranchisor.ID IS NULL" & vbCrLf &
                " OR tblInvoiceItemsFranchisor.ID IS NULL)" & vbCrLf
            Else
                pSql &= " WHERE tblInvoiceFranchisor.ID IS NULL" & vbCrLf
            End If
            pSql &= " AND tblInvoiceFranchisee.IsCancelled = 0" & vbCrLf &
            " AND (tblInvoiceFranchisee.InvoiceNumber <> 0" & vbCrLf &
            "   OR ISNULL(tblInvoiceFranchisee.PurchaseOrderNo, '') <> ''" & vbCrLf &
            ") AND tblInvoiceFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " -- AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) >= " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " AND tblInvoiceFranchisee.IsTestOnly = 0" & vbCrLf &
            " ORDER BY tblInvoiceFranchisee.LastUpdated" 'do not include data integrty test invoices! important- added by janine
            If DetectInvoicesWithNoItems Then
                pSql &= " DESC"
            End If
            pDT = mProData.Execute(pSql)
            AppendToLog("vb28WMR343CN! GetCopyableInvoices", mFolder)

            Return pDT

        End Function

        Private Function GetInvoiceItems(vInvoiceID As String, vFranchiseeDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = "SELECT ID" & vbCrLf &
            ",ProductID" & vbCrLf &
            ",Quantity" & vbCrLf &
            ",OrderedQuantity" & vbCrLf &
            ",ActualQuantity" & vbCrLf &
            ",UnitMeasure" & vbCrLf &
            ",TermDiscount" & vbCrLf &
            ",UnitPrice" & vbCrLf &
            ",SubTotal" & vbCrLf &
            ",Cost" & vbCrLf &
            ",PriceLevelID" & vbCrLf &
            ",TaxPercent" & vbCrLf &
            ",TaxAmount" & vbCrLf &
            ",TaxSales" & vbCrLf &
            ",DiscountAmount" & vbCrLf &
            ",DiscountSales" & vbCrLf &
            ",SubcriptionDuration" & vbCrLf &
            ",SubscriptionUnitMeasure" & vbCrLf &
            ",SubcriptionDiscount" & vbCrLf &
            ",ModifierGroupID" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransInvoiceItems TransInvoiceItems" &
            " WHERE InvoiceID = " & ReadyString(vInvoiceID)
            pDT = mProData.Execute(pSql)

            Return pDT

        End Function

        Private Function GetInvoiceModifiers(vInvoiceID As String, vFranchiseeDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = "SELECT TransInvoiceItemsModifier.ID" & vbCrLf &
            ",TransInvoiceItemsModifier.InvoiceItemID" & vbCrLf &
            ",TransInvoiceItemsModifier.ItemModifierID" & vbCrLf &
            ",TransInvoiceItemsModifier.ModifierID" & vbCrLf &
            ",TransInvoiceItemsModifier.InputQty" & vbCrLf &
            ",TransInvoiceItemsModifier.OrderedQty" & vbCrLf &
            ",TransInvoiceItemsModifier.ModifierPrice" & vbCrLf &
            ",TransInvoiceItemsModifier.SubTotal" & vbCrLf &
            ",TransInvoiceItemsModifier.OtherDescription" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransInvoiceItems TransInvoiceItems" &
            " JOIN " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransInvoiceItemsModifier TransInvoiceItemsModifier ON TransInvoiceItemsModifier.InvoiceItemID = TransInvoiceItems.ID" &
            " WHERE TransInvoiceItems.InvoiceID = " & ReadyString(vInvoiceID)
            pDT = mProData.Execute(pSql)

            Return pDT

        End Function

        Private Sub CopyInvoice(ByVal vDR As DataRow,
        ByVal vDtItems As DataTable,
        ByVal vDtModifiers As DataTable,
        ByVal vFranchiseeOrgID As String, ByVal vFranchiseeFowstID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String,
        vIsPo As Boolean)
            ' PARAMS:
            '   vDR = TransInvoice
            '   vDtItems = TransInvoiceItems

            Dim pIU As InsertUpdate
            Dim pSql As String
            Dim pInvoiceExist As Boolean

            pIU = New InsertUpdate(EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoice")
            pSql = "SELECT COUNT(*)" & vbCrLf &
            " FROM " & pIU.TableName & vbCrLf &
            " WHERE ID = " & ReadyString(CStr2(vDR("ID")))
            Dim pExistingCount = CInt2(mProData.ExecuteScalar(pSql))
            If pExistingCount > 0 Then
                AppendToLog("vb2CCRYJEEVZ! CopyInvoice already exist" & vbCrLf &
                "ID=" & CStr2(vDR("ID")), , "FranchiseeDataExporter")
                If DetectInvoicesWithNoItems = False Then
                    Return
                End If
                pInvoiceExist = True
            End If

            DataRowToInsertUpdate(vDR, pIU)
            'pIU("FranchiseeOrgID") = ReadyString(vFranchiseeOrgFowstID) 'commented by janine nov 5. do not save in this field anymore. save to FowstID directly
            pIU("InvoiceNumber") = 0 ' 2022_09_14__11_35_07_7045 gina zero for some reason
            If vIsPo Then
                pIU("FranchiseeSalesOrderNo") = vDR("PurchaseOrderNo")
                pIU("FowstID") = ReadyString(vFranchisorFowstID)
            Else
                pIU("FowstID") = ReadyString(vFranchiseeFowstID)
                pIU("CustomerID") = ReadyString(vFranchiseeOrgID)
                pIU("BillingAddressID") = ReadyString(vDR("FowstID"))
                pIU("DeliveryAddressID") = ReadyString(vDR("FowstID"))
            End If
            'edited. new column added to TransInvoice - janine nov28
            pIU("FranchiseeInvoiceNumber") = vDR.Item("InvoiceNumber")
            'pIU("SalesOrderNo") = "ISNULL((SELECT MAX(TransInvoice.SalesOrderNo) " & vbCrLf &
            '    " FROM " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoice TransInvoice" & vbCrLf &
            '    " WHERE TransInvoice.IsCancelled = 0" & vbCrLf &
            '    " And TransInvoice.FowstID = " & ReadyString(vFranchisorFowstID) & vbCrLf &
            '    "), 0) + 1"
            mProData.BeginTransaction()
            Try
                If pInvoiceExist = False Then
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                End If
                CopyInvoiceItems(vDtItems, CStr2(vDR("ID")), vFranchisorDatabase, vIsPo)
                CopyInvoiceModifiers(vDtModifiers, vFranchisorDatabase)

                Try
                    Dim pCurrentDatabase = mBusiness.ProData.GetCurrentDatabase()
                    Console.WriteLine("pCurrentDatabase = " & pCurrentDatabase)
                    Dim pCopyDeviceResult = mBusiness.LibraryDevice.CopyToDatabase(CStr2(vDR("DeviceID")), vFranchisorDatabase)
                    Dim pCopyDeviceFowstResult = mBusiness.LibraryDeviceFowst.CopyToDatabase(CStr2(vDR("DeviceID")), vFranchisorDatabase)
                    If Now.Date = New Date(2022, 11, 3) Then
                        AppendToLog("DeviceID: " & CStr2(vDR("DeviceID")) & vbCrLf &
                        "pCopyDeviceResult: " & pCopyDeviceResult & vbCrLf &
                        "pCopyDeviceFowstResult: " & pCopyDeviceFowstResult, , "CopyDevice vbWNYUC667XS")
                    End If
                Catch ex As Exception
                    If ex.Message.Contains("Cannot insert duplicate key row in object 'dbo.LibraryDevice' with unique index 'LibraryDevice_uq'.") Then
                        If mHasIgnoreError = False Then
                            AppendToDebug("vb3BHVV428MY! This error is being ignored since 2024_04_26__09_13_20_4380. " & ex.Message, True, ex, True)
                            mHasIgnoreError = True
                        End If
                    Else
                        AppendToDebug("vb7F24TGP5MK! " & ex.Message, True, ex, True)
                        Throw
                    End If
                End Try

                mProData.CommitTransaction()
                'pSql = mProData.ExecuteScalar("SELECT SalesOrderNo FROM " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoice TransInvoice WHERE ID = " & ReadyString(vDR("ID"))) ' debug only
                'mProData.RollbackTransaction(False, True) ' debug only
            Catch ex As Exception
                mProData.RollbackTransaction(False, True)
                Throw
            End Try

        End Sub

        Private Sub CopyInvoiceModifiers(vDtModifiers As DataTable, vFranchisorDatabase As String)

            Dim pIU As InsertUpdate
            Dim pSql As String

            pIU = New InsertUpdate(EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoiceItemsModifier")
            'mProData.BeginTransaction()
            Try
                For Each pDR As DataRow In vDtModifiers.Rows
                    If DetectInvoicesWithNoItems Then
                        pSql = "SELECT COUNT(*)" & vbCrLf &
                        " FROM " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoiceItemsModifier" & vbCrLf &
                        " WHERE ID = " & ReadyString(pDR("ID"))
                        Dim pRecords = mProData.ExecuteScalar(pSql)
                        If CInt2(pRecords) > 0 Then
                            Continue For
                        End If
                    End If
                    DataRowToInsertUpdate(pDR, pIU)
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                Next pDR

                'mProData.CommitTransaction()
            Catch ex As Exception
                mProData.RollbackTransaction(False, True)
                Throw
            End Try

        End Sub

        Private Sub CopyInvoiceItems(vDtItems As DataTable, vInvoiceID As String, vFranchisorDatabase As String, vIsPo As Boolean)

            Dim pIU As InsertUpdate
            Dim pSql As String

            pIU = New InsertUpdate(EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoiceItems")
            pIU("InvoiceID") = ReadyString(vInvoiceID)
            'mProData.BeginTransaction()
            Try
                For Each pDR As DataRow In vDtItems.Rows
                    If DetectInvoicesWithNoItems Then
                        pSql = "SELECT COUNT(*)" & vbCrLf &
                        " FROM " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransInvoiceItems" & vbCrLf &
                        " WHERE ID = " & ReadyString(pDR("ID"))
                        Dim pRecords = mProData.ExecuteScalar(pSql)
                        If CInt2(pRecords) > 0 Then
                            Continue For
                        End If
                    End If

                    DataRowToInsertUpdate(pDR, pIU)
                    pIU("DiscountAmount") = CDec2(pDR("DiscountAmount"))
                    If vIsPo = False Then
                        pIU("OrderedQuantity") = pIU("Quantity")
                        pIU.Remove("Quantity")
                    End If
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                Next pDR

                'mProData.CommitTransaction()
            Catch ex As Exception
                'mProData.RollbackTransaction(False, True)
                Throw
            End Try

        End Sub

        Private Function CopySalesReturns(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String) As DataTable

            Dim pDtMemos As DataTable
            Dim pDtItems As DataTable
            Dim pSW = Stopwatch.StartNew()

            Do
                pDtMemos = GetCopyableSalesReturns(
                vFranchiseeDatabase:=vFranchiseeDatabase,
                vFranchiseeFowstID:=vFranchiseeFowstID,
                vFranchisorDatabase:=vFranchisorDatabase)
                AppendToLog("vbJ52J59MEP2! GetCopyableSalesReturns " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
            "Rows = " & pDtMemos.Rows.Count, mFolder)

                pSW.Restart()
                For Each pDrMemo As DataRow In pDtMemos.Rows
                    pDtItems = GetTransMemoSalesReturn(CStr2(pDrMemo("ID")), vFranchiseeDatabase)
                    Dim pDtSalesReturnLedger = GetTransSalesReturnLedger(CStr2(pDrMemo("ID")), vFranchiseeDatabase)
                    CopyMemo(
                    vDR:=pDrMemo,
                    vDtTransMemoSalesReturn:=pDtItems,
                    vDtTransSalesReturnLedger:=pDtSalesReturnLedger,
                    vFranchiseeOrgID:=vFranchiseeOrgID,
                    vFranchiseeFowstID:=vFranchiseeFowstID,
                    vFranchisorDatabase:=vFranchisorDatabase,
                    vFranchisorFowstID:=vFranchisorFowstID)
                Next pDrMemo
                AppendToLog("vb55LYJ59V69! CopySalesReturns " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDtMemos.Rows.Count, mFolder)
                If pDtMemos.Rows.Count >= mTop Then
                    AppendToLog("vb7FKLYZESU8! repeating copy - CopyInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDtMemos.Rows.Count, mFolder)
                Else
                    Exit Do
                End If
            Loop

            Return pDtMemos

        End Function

        Private Function GetTransMemoSalesReturn(vMemoID As String, vFranchiseeDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = "SELECT *" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransMemoSalesReturn TransMemoSalesReturn" &
            " WHERE MemoID = " & ReadyString(vMemoID)
            pDT = mProData.Execute(pSql)

            Return pDT

        End Function

        Private Function GetTransSalesReturnLedger(vMemoID As String, vFranchiseeDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = "SELECT *" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransSalesReturnLedger TransSalesReturnLedger" &
            " WHERE SourceMemoID = " & ReadyString(vMemoID)
            pDT = mProData.Execute(pSql)

            Return pDT

        End Function

        Private Sub CopyMemo(ByVal vDR As DataRow,
        ByVal vDtTransMemoSalesReturn As DataTable,
        ByVal vDtTransSalesReturnLedger As DataTable,
        ByVal vFranchiseeOrgID As String, ByVal vFranchiseeFowstID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String)
            ' PARAMS:
            '   vDR = TransInvoice
            '   vDtItems = TransInvoiceItems

            Dim pIU As InsertUpdate
            Dim pSql As String

            pIU = New InsertUpdate(EscapeSqlName(vFranchisorDatabase) & ".dbo.TransMemo")
            DataRowToInsertUpdate(vDR, pIU)
            mProData.BeginTransaction()
            Try
                pIU.Remove("MemoNumber")
                'mProData.SetInsertIdentity(pIU.TableName, True, True) ' not working
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
                CopyTransMemoSalesReturn(vDtTransMemoSalesReturn, vFranchisorDatabase)
                CopyTransSalesReturnLedger(vDtTransSalesReturnLedger, vFranchisorDatabase)
                mProData.CommitTransaction()
            Catch ex As Exception
                mProData.RollbackTransaction(False, True)
                Throw New ApplicationException(ex.Message & vbCrLf &
                "Table: " & pIU.TableName, ex)
            End Try

        End Sub

        Private Sub CopyTransMemoSalesReturn(vDtItems As DataTable, vFranchisorDatabase As String)

            Dim pIU As InsertUpdate
            Dim pSql As String

            pIU = New InsertUpdate(EscapeSqlName(vFranchisorDatabase) & ".dbo.TransMemoSalesReturn")
            mProData.BeginTransaction()
            Try
                For Each pDR As DataRow In vDtItems.Rows
                    DataRowToInsertUpdate(pDR, pIU)
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                Next pDR
                mProData.CommitTransaction()
            Catch ex As Exception
                mProData.RollbackTransaction(False, True)
                Throw
            End Try

        End Sub

        Private Sub CopyTransSalesReturnLedger(vDtItems As DataTable, vFranchisorDatabase As String)

            Dim pIU As InsertUpdate
            Dim pSql As String

            pIU = New InsertUpdate(EscapeSqlName(vFranchisorDatabase) & ".dbo.TransSalesReturnLedger")
            mProData.BeginTransaction()
            Try
                For Each pDR As DataRow In vDtItems.Rows
                    DataRowToInsertUpdate(pDR, pIU)
                    pSql = pIU.GetInsert
                    mProData.ExecuteSQL(pSql)
                Next pDR
                mProData.CommitTransaction()
            Catch ex As Exception
                mProData.RollbackTransaction(False, True)
                Throw
            End Try

        End Sub

        Private Function GetCopyableSalesReturns(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
        vFranchisorDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT TOP " & mTop & " tblMemoFranchisee.*" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.TransMemo tblMemoFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.TransMemo tblMemoFranchisor ON tblMemoFranchisor.ID = tblMemoFranchisee.ID" & vbCrLf &
            " WHERE tblMemoFranchisor.ID IS NULL" & vbCrLf &
            " AND tblMemoFranchisee.MemoNumber <> 0" & vbCrLf &
            " AND tblMemoFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblMemoFranchisee.MemoDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " -- AND CONVERT(date, tblMemoFranchisee.MemoDate) >= " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " ORDER BY tblMemoFranchisee.LastUpdated" 'do not include data integrty test invoices! important- added by janine
            pDT = mProData.Execute(pSql)

            Return pDT

        End Function

        Private Function GetCopyableCacheInventoryMovement(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
        vFranchisorDatabase As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT TOP " & mTop & " tblDataFranchisee.*" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.CacheInventoryMovement tblDataFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInventoryMovement tblDataFranchisor ON tblDataFranchisor.FowstID = tblDataFranchisee.FowstID" & vbCrLf &
            " AND tblDataFranchisor.ProductID = tblDataFranchisee.ProductID" & vbCrLf &
            " AND tblDataFranchisor.TransactionDate = tblDataFranchisee.TransactionDate" & vbCrLf &
            " WHERE (tblDataFranchisor.FowstID IS NULL -- the field is not important. you can pick any field" & vbCrLf &
            "   OR ISNULL(tblDataFranchisor.Delivery, 0) <> ISNULL(tblDataFranchisee.Delivery, 0)" & vbCrLf &
            "   OR ISNULL(tblDataFranchisor.TransIn, 0) <> ISNULL(tblDataFranchisee.TransIn, 0)" & vbCrLf &
            "   OR ISNULL(tblDataFranchisor.TransOut, 0) <> ISNULL(tblDataFranchisee.TransOut, 0)" & vbCrLf &
            "   OR ISNULL(tblDataFranchisor.EndingBalance, 0) <> ISNULL(tblDataFranchisee.EndingBalance, 0)" & vbCrLf &
            "   OR ISNULL(tblDataFranchisor.Usage, 0) <> ISNULL(tblDataFranchisee.Usage, 0)" & vbCrLf &
            " )" & vbCrLf &
            " AND tblDataFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblDataFranchisee.TransactionDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " -- AND CONVERT(date, tblDataFranchisee.TransactionDate) >= " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " ORDER BY tblDataFranchisee.TransactionDate"
            pDT = mProData.Execute(pSql)
            If pDT.Rows.Count = 0 Then
                'AppendToLog("vb8FWLWSCDA8! pDT.Rows.Count=" & pDT.Rows.Count & vbCrLf & pSql)
            Else
                AppendToLog("vb6R5GFB8SX7! pDT.Rows.Count=" & pDT.Rows.Count & vbCrLf & pSql)
            End If
            'pDT.Rows.Clear()

            Return pDT

        End Function

        Private Function GetCopyableCacheInvoices(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
            vFranchisorDatabase As String,
            vFranchiseGroupID As String,
            vGroupName As String,
            vRegionName As String,
            vRegionID As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT TOP " & mTop & " tblInvoiceFranchisee.*" & vbCrLf &
            "/*" & vbCrLf &
            ",tblInvoiceFranchisor.FranchiseGroupID AS Franchisor_FranchiseGroupID" & vbCrLf &
            ",tblInvoiceFranchisor.GroupName AS Franchisor_GroupName" & vbCrLf &
            ",tblInvoiceFranchisor.RegionName AS Franchisor_RegionName" & vbCrLf &
            ",tblInvoiceFranchisor.FromFranchisee AS Franchisor_FromFranchisee" & vbCrLf &
            ",tblInvoiceFranchisor.RegionID AS Franchisor_RegionID" & vbCrLf &
            "*/" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.CacheInvoices tblInvoiceFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoices tblInvoiceFranchisor ON tblInvoiceFranchisor.ID = tblInvoiceFranchisee.ID" & vbCrLf &
            " WHERE (tblInvoiceFranchisor.ID IS NULL" & vbCrLf
            If String.IsNullOrWhiteSpace(vFranchiseGroupID) = False Then
                pSql &= " OR tblInvoiceFranchisor.FranchiseGroupID <> " & ReadyString(vFranchiseGroupID) & vbCrLf
            End If
            pSql &= " OR ISNULL(tblInvoiceFranchisor.GroupName, '') <> " & ReadyString(vGroupName) & vbCrLf &
            " OR ISNULL(tblInvoiceFranchisor.RegionName, '') <> " & ReadyString(vRegionName) & vbCrLf &
            " OR ISNULL(tblInvoiceFranchisor.FromFranchisee, 0) = 0 " & vbCrLf &
            " OR ISNULL(CONVERT(varchar(36), tblInvoiceFranchisor.RegionID), '') <> " & ReadyString(ReadyGuid(vRegionID)) & vbCrLf &
            ") AND tblInvoiceFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " -- AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) >= " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " /* " & vbCrLf &
            " AND (tblInvoiceFranchisor.ID IS NULL" & vbCrLf &
            " OR tblInvoiceFranchisee.LastUpdated <> tblInvoiceFranchisor.LastUpdated " & vbCrLf &
            ")" & vbCrLf &
            " */ " & vbCrLf &
            " ORDER BY tblInvoiceFranchisee.LastUpdated"
            If Now.Date = #7/24/2023# Then
                AppendToLog("vbAQAM528U87! vFranchiseGroupID = " & vFranchiseGroupID)
                Try
                    pDT = mProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        Dim pFilename = AppPath & "temp\FranchiseeDataExporter\CopyableCacheInvoices_" & DateTimeStamp() & ".xlsx"
                        CreateFolder(pFilename)
                        TableToExcelFile(pDT, "", "", pFilename)
                    End If
                Catch ex As ProSqlException
                    AppendToLog("vbGSACDCJXNZ! " & ex.Message & vbCrLf &
                    ex.Sql)
                Catch ex As Exception
                    AppendToDebug("vbGAYQWX48EU! " & ex.Message)
                End Try
            End If

            'pSql = " SELECT tblInvoiceFranchisee.*" & vbCrLf &
            '    " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.CacheInvoices tblInvoiceFranchisee" & vbCrLf &
            '    " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoices tblInvoiceFranchisor ON tblInvoiceFranchisor.ID = tblInvoiceFranchisee.ID" & vbCrLf &
            '    " WHERE (tblInvoiceFranchisor.ID IS NULL" & vbCrLf &
            '    " OR ISNULL(tblInvoiceFranchisee.GrossSalesNetOfVATExemptions, 0) <> ISNULL(tblInvoiceFranchisor.GrossSalesNetOfVATExemptions, 0)" & vbCrLf &
            '    " OR ISNULL(tblInvoiceFranchisee.FOWSTName, '') <> ISNULL(tblInvoiceFranchisor.FOWSTName, '')" & vbCrLf &
            '    " /* " & vbCrLf &
            '    " OR ISNULL(tblInvoiceFranchisee.GroupName, '') <> ISNULL(tblInvoiceFranchisor.GroupName, '')" & vbCrLf &
            '    " OR ISNULL(CONVERT(varchar(36), tblInvoiceFranchisee.FranchiseGroupID), '') <> ISNULL(CONVERT(varchar(36), tblInvoiceFranchisor.FranchiseGroupID), '')" & vbCrLf &
            '    " */ " & vbCrLf &
            '    ") AND tblInvoiceFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            '    " AND tblInvoiceFranchisee.InvoiceDate >= " & ReadyString(mStartDate) & vbCrLf &
            '    " /* " & vbCrLf &
            '    " AND (tblInvoiceFranchisor.ID IS NULL" & vbCrLf &
            '    " OR tblInvoiceFranchisee.LastUpdated <> tblInvoiceFranchisor.LastUpdated " & vbCrLf &
            '    ")" & vbCrLf &
            '    " */ " & vbCrLf &
            '    " ORDER BY tblInvoiceFranchisee.LastUpdated"

            'AppendToLog("vb48VMSUTAUD! vFranchiseeDatabase=" & vFranchiseeDatabase & vbCrLf &
            '    "vFranchisorDatabase=" & vFranchisorDatabase & vbCrLf &
            '    pSql, , "FranchiseeDataExporter")
            If pDT Is Nothing Then
                pDT = mProData.Execute(pSql)
            End If
            'If pDT.Rows.Count = 0 Then
            '    AppendToLog("vbZQB9GANR4E! pDT.Rows.Count=" & pDT.Rows.Count, , "FranchiseeDataExporter")
            'Else
            '    AppendToLog("vbXMVHN5P947! pDT.Rows.Count=" & pDT.Rows.Count, , "FranchiseeDataExporter")
            'End If
            Return pDT

        End Function

        Private Function CopyCacheInvoices(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String,
        ByVal vFranchiseGroupID As String,
        ByVal vFranchisorGroupName As String,
        ByVal vRegionalGroupID As String,
        ByVal vRegionName As String) As Integer

            Dim pDT As DataTable
            Dim pSW = Stopwatch.StartNew()
            Dim pOut As Integer

            Do
                LastMessage = "GetCopyableCacheInvoices"
                pDT = GetCopyableCacheInvoices(
                    vFranchiseeDatabase:=vFranchiseeDatabase,
                    vFranchiseeFowstID:=vFranchiseeFowstID,
                    vFranchisorDatabase:=vFranchisorDatabase,
                    vFranchiseGroupID:=vFranchiseGroupID,
                    vGroupName:=vFranchisorGroupName,
                    vRegionName:=vRegionName,
                    vRegionID:=vRegionalGroupID)
                AppendToLog("vbU6M4WAHTP5! GetCopyableCacheInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                    "Rows = " & pDT.Rows.Count, mFolder)
                pOut += pDT.Rows.Count
                LastMessage = "GetCopyableCacheInvoices count = " & pDT.Rows.Count
                pSW.Restart()
                For Each pDR As DataRow In pDT.Rows
                    LastMessage = "Saving to CacheInvoices " & (pDT.Rows.IndexOf(pDR) + 1) & " of " & pDT.Rows.Count
                    Dim pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoices"
                    If String.IsNullOrWhiteSpace(vFranchiseGroupID) = False Then
                        pIU("FranchiseGroupID") = ReadyString(vFranchiseGroupID)
                    End If
                    pIU("GroupName") = ReadyString(vFranchisorGroupName)
                    If String.IsNullOrWhiteSpace(vRegionalGroupID) = False Then
                        pIU("RegionID") = ReadyString(vRegionalGroupID)
                    End If
                    pIU("RegionName") = ReadyString(vRegionName)
                    pIU("FromFranchisee") = 1
                    Dim pSql = pIU.GetUpdate("ID = " & ReadyString(pDR("ID")))
                    'AppendToLog("vb6Z5BG7QK56! " & pSql, , "FranchiseeDataExporter")

                    Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
                    If pAffectedRecords = 0 Then
                        pSql = pIU.GetInsert()
                        mProData.ExecuteSQL(pSql)
                    End If
                Next
                AppendToLog("vb3ZSRWQ8X7K! CopyCacheInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                If pDT.Rows.Count >= mTop Then
                    AppendToLog("vbYM7LNX7PLL! repeating copy - CopyCacheInvoices " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                Else
                    Exit Do
                End If
            Loop

            Return pOut

        End Function

        Private Function GetCopyableCacheInvoiceDaily(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
        vFranchisorDatabase As String,
        vFranchiseGroupID As String,
        vGroupName As String,
        vRegionName As String,
        vRegionID As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT TOP " & mTop & " tblInvoiceFranchisee.*" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.CacheInvoicesDaily tblInvoiceFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoicesDaily tblInvoiceFranchisor ON tblInvoiceFranchisor.ID = tblInvoiceFranchisee.ID" & vbCrLf &
            " WHERE (tblInvoiceFranchisor.InvoiceDate IS NULL -- any required field will do" & vbCrLf
            If String.IsNullOrWhiteSpace(vFranchiseGroupID) = False Then
                pSql &= " OR tblInvoiceFranchisor.FranchiseGroupID <> " & ReadyString(vFranchiseGroupID) & vbCrLf
            End If
            pSql &= " OR ISNULL(tblInvoiceFranchisor.GroupName, '') <> " & ReadyString(vGroupName) & vbCrLf &
            " OR ISNULL(tblInvoiceFranchisor.RegionName, '') <> " & ReadyString(vRegionName) & vbCrLf &
            " OR ISNULL(tblInvoiceFranchisor.FromFranchisee, 0) = 0 " & vbCrLf &
            " OR ISNULL(CONVERT(varchar(36), tblInvoiceFranchisor.RegionID), '') <> " & ReadyString(ReadyGuid(vRegionID)) & vbCrLf &
            ") AND tblInvoiceFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " ORDER BY tblInvoiceFranchisee.InvoiceDate"
            If Now.Date = #7/25/2023# Then
                AppendToLog("vbAQAM528U87! vFranchiseGroupID = " & vFranchiseGroupID)
                Try
                    pDT = mProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        Dim pFilename = AppPath & "temp\FranchiseeDataExporter\GetCopyableCacheInvoiceDaily_" & DateTimeStamp() & ".xlsx"
                        CreateFolder(pFilename)
                        TableToExcelFile(pDT, "", "", pFilename)
                    End If
                Catch ex As ProSqlException
                    AppendToLog("vbZXFC7LYK9X! " & ex.Message & vbCrLf &
                    ex.Sql)
                Catch ex As Exception
                    AppendToDebug("vb8EQXRHGFV6! " & ex.Message)
                End Try
            End If

            If pDT Is Nothing Then
                pDT = mProData.Execute(pSql)
            End If

            Return pDT

        End Function

        Private Function CopyCacheInvoiceDaily(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String,
        ByVal vFranchiseGroupID As String,
        ByVal vFranchisorGroupName As String,
        ByVal vRegionalGroupID As String,
        ByVal vRegionName As String) As Integer

            Dim pDT As DataTable
            Dim pSW = Stopwatch.StartNew()
            Dim pOut As Integer

            Do
                LastMessage = "GetCopyableCacheInvoiceDaily"
                pDT = GetCopyableCacheInvoiceDaily(
                vFranchiseeDatabase:=vFranchiseeDatabase,
                vFranchiseeFowstID:=vFranchiseeFowstID,
                vFranchisorDatabase:=vFranchisorDatabase,
                vFranchiseGroupID:=vFranchiseGroupID,
                vGroupName:=vFranchisorGroupName,
                vRegionName:=vRegionName,
                vRegionID:=vRegionalGroupID)
                LastMessage = "GetCopyableCacheInvoiceDaily count = " & pDT.Rows.Count
                AppendToLog("vb94WHR2MQDJ! GetCopyableCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                    "Rows = " & pDT.Rows.Count, mFolder)
                pOut += pDT.Rows.Count
                pSW.Restart()
                For Each pDR As DataRow In pDT.Rows
                    LastMessage = "Saving to CacheInvoiceDaily " & (pDT.Rows.IndexOf(pDR) + 1) & " of " & pDT.Rows.Count
                    Dim pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoicesDaily"
                    If pDT.Columns.Contains("InvoiceNo") Then
                        pIU.Remove("InvoiceNo")
                    End If
                    If String.IsNullOrWhiteSpace(vFranchiseGroupID) = False Then
                        pIU("FranchiseGroupID") = ReadyString(vFranchiseGroupID)
                    End If
                    pIU("GroupName") = ReadyString(vFranchisorGroupName)
                    If String.IsNullOrWhiteSpace(vRegionalGroupID) = False Then
                        pIU("RegionID") = ReadyString(vRegionalGroupID)
                    End If
                    pIU("RegionName") = ReadyString(vRegionName)
                    pIU("FromFranchisee") = 1
                    Dim pSql = pIU.GetUpdate("FowstID = " & ReadyString(pDR("FowstID")) & vbCrLf &
                    " AND InvoiceDate = " & ReadyString(CDate(pDR("InvoiceDate")).ToString(cGlobal.FormatDateSql)))
                    'AppendToLog("vb6Z5BG7QK56! " & pSql, , "FranchiseeDataExporter")

                    Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
                    If pAffectedRecords = 0 Then
                        pSql = pIU.GetInsert()
                        mProData.ExecuteSQL(pSql)
                    End If
                Next
                AppendToLog("vb4ML7LN83W9! CopyCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                If pDT.Rows.Count >= mTop Then
                    AppendToLog("vb7PQFPHECG3! repeating copy - CopyCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                Else
                    Exit Do
                End If
            Loop

            Return pOut

        End Function

        Private Function GetCopyableCacheInvoiceFinishedProducts(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
        vFranchisorDatabase As String,
        vFranchiseGroupID As String,
        vGroupName As String,
        vRegionName As String,
        vRegionID As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT TOP " & mTop & " tblInvoiceFranchisee.*" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.CacheInvoiceFinishedProducts tblInvoiceFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoiceFinishedProducts tblInvoiceFranchisor" & vbCrLf &
            " ON tblInvoiceFranchisor.FowstID = tblInvoiceFranchisee.FowstID" & vbCrLf &
            " AND tblInvoiceFranchisor.InvoiceDate = tblInvoiceFranchisee.InvoiceDate" & vbCrLf &
            " AND tblInvoiceFranchisor.ProductID = tblInvoiceFranchisee.ProductID" & vbCrLf &
            " WHERE (tblInvoiceFranchisor.InvoiceDate Is NULL -- any required field will do" & vbCrLf
            If String.IsNullOrWhiteSpace(vFranchiseGroupID) = False Then
                pSql &= " OR tblInvoiceFranchisor.FranchiseGroupID <> " & ReadyString(vFranchiseGroupID) & vbCrLf
            End If
            pSql &= " Or ISNULL(tblInvoiceFranchisor.GroupName, '') <> " & ReadyString(vGroupName) & vbCrLf &
            " OR ISNULL(tblInvoiceFranchisor.FromFranchisee, 0) = 0 " & vbCrLf &
            ") AND tblInvoiceFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " ORDER BY tblInvoiceFranchisee.InvoiceDate"
            If Now.Date = #7/25/2023# Then
                AppendToLog("vbAQAM528U87! vFranchiseGroupID = " & vFranchiseGroupID)
                Try
                    pDT = mProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        Dim pFilename = AppPath & "temp\FranchiseeDataExporter\GetCacheInvoiceFinishedProducts_" & DateTimeStamp() & ".xlsx"
                        CreateFolder(pFilename)
                        TableToExcelFile(pDT, "", "", pFilename)
                    End If
                Catch ex As ProSqlException
                    AppendToLog("vbZXFC7LYK9X! " & ex.Message & vbCrLf &
                    ex.Sql)
                Catch ex As Exception
                    AppendToDebug("vb8EQXRHGFV6! " & ex.Message)
                End Try
            End If

            If pDT Is Nothing Then
                pDT = mProData.Execute(pSql)
            End If

            Return pDT

        End Function

        Private Function CopyCacheInvoiceFinishedProducts(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String,
        ByVal vFranchiseGroupID As String,
        ByVal vFranchisorGroupName As String,
        ByVal vRegionalGroupID As String,
        ByVal vRegionName As String) As Integer

            Dim pDT As DataTable
            Dim pSW = Stopwatch.StartNew()
            Dim pOut As Integer

            Do
                LastMessage = "GetCopyableCacheInvoiceFinishedProducts"
                pDT = GetCopyableCacheInvoiceFinishedProducts(
                vFranchiseeDatabase:=vFranchiseeDatabase,
                vFranchiseeFowstID:=vFranchiseeFowstID,
                vFranchisorDatabase:=vFranchisorDatabase,
                vFranchiseGroupID:=vFranchiseGroupID,
                vGroupName:=vFranchisorGroupName,
                vRegionName:=vRegionName,
                vRegionID:=vRegionalGroupID)
                LastMessage = "GetCopyableCacheInvoiceFinishedProducts count = " & pDT.Rows.Count
                AppendToLog("vb! GetCopyableCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
            "Rows = " & pDT.Rows.Count, mFolder)
                pOut += pDT.Rows.Count
                pSW.Restart()
                For Each pDR As DataRow In pDT.Rows
                    LastMessage = "Saving to CacheInvoiceFinishedProducts " & (pDT.Rows.IndexOf(pDR) + 1) & " of " & pDT.Rows.Count
                    Dim pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoiceFinishedProducts"
                    If pDT.Columns.Contains("InvoiceNo") Then
                        pIU.Remove("InvoiceNo")
                    End If
                    pIU("FranchiseGroupID") = ReadyString(vFranchiseGroupID)
                    pIU("GroupName") = ReadyString(vFranchisorGroupName)
                    pIU("FromFranchisee") = 1
                    Dim pSql = pIU.GetUpdate("FowstID = " & ReadyString(pDR("FowstID")) & vbCrLf &
                    " AND InvoiceDate = " & ReadyString(CDate(pDR("InvoiceDate")).ToString(cGlobal.FormatDateSql)) & vbCrLf &
                    " AND ProductID = " & ReadyString(pDR("ProductID")))
                    'AppendToLog("vb6Z5BG7QK56! " & pSql, , "FranchiseeDataExporter")

                    Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
                    If pAffectedRecords = 0 Then
                        pSql = pIU.GetInsert()
                        mProData.ExecuteSQL(pSql)
                    End If
                Next
                AppendToLog("vb4ML7LN83W9! CopyCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                If pDT.Rows.Count >= mTop Then
                    AppendToLog("vb7PQFPHECG3! repeating copy - CopyCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                Else
                    Exit Do
                End If
            Loop

            Return pOut

        End Function

        Private Function GetCopyableCacheInvoiceFlavors(vFranchiseeDatabase As String, vFranchiseeFowstID As String,
        vFranchisorDatabase As String,
        vFranchiseGroupID As String,
        vGroupName As String,
        vRegionName As String,
        vRegionID As String) As DataTable

            Dim pDT As DataTable
            Dim pSql As String

            pSql = " SELECT TOP " & mTop & " tblInvoiceFranchisee.*" & vbCrLf &
            " FROM " & EscapeSqlName(vFranchiseeDatabase) & ".dbo.CacheInvoiceFlavors tblInvoiceFranchisee" & vbCrLf &
            " LEFT JOIN " & EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoiceFlavors tblInvoiceFranchisor" & vbCrLf &
            " ON tblInvoiceFranchisor.FowstID = tblInvoiceFranchisee.FowstID" & vbCrLf &
            " AND tblInvoiceFranchisor.InvoiceDate = tblInvoiceFranchisee.InvoiceDate" & vbCrLf &
            " AND tblInvoiceFranchisor.ModifierProductID = tblInvoiceFranchisee.ModifierProductID" & vbCrLf &
            " AND tblInvoiceFranchisor.ModifierUnitMeasure = tblInvoiceFranchisee.ModifierUnitMeasure" & vbCrLf &
            " WHERE (tblInvoiceFranchisor.InvoiceDate Is NULL -- any required field will do" & vbCrLf
            If String.IsNullOrWhiteSpace(vFranchiseGroupID) = False Then
                pSql &= " Or tblInvoiceFranchisor.FranchiseGroupID <> " & ReadyString(vFranchiseGroupID) & vbCrLf
            End If
            pSql &= " Or ISNULL(tblInvoiceFranchisor.GroupName, '') <> " & ReadyString(vGroupName) & vbCrLf &
            ") AND tblInvoiceFranchisee.FowstID = " & ReadyString(vFranchiseeFowstID) & vbCrLf &
            " AND CONVERT(date, tblInvoiceFranchisee.InvoiceDate) BETWEEN " & ReadyString(StartDate.ToString(cGlobal.FormatDateSql)) & " AND " & ReadyString(EndDate.ToString(cGlobal.FormatDateSql)) & vbCrLf &
            " ORDER BY tblInvoiceFranchisee.InvoiceDate"
            If Now.Date = #7/25/2023# Then
                AppendToLog("vbAQAM528U87! vFranchiseGroupID = " & vFranchiseGroupID)
                Try
                    pDT = mProData.Execute(pSql)
                    If pDT.Rows.Count > 0 Then
                        Dim pFilename = AppPath & "temp\FranchiseeDataExporter\GetCacheInvoiceFlavors_" & DateTimeStamp() & ".xlsx"
                        CreateFolder(pFilename)
                        TableToExcelFile(pDT, "", "", pFilename)
                    End If
                Catch ex As ProSqlException
                    AppendToLog("vbZXFC7LYK9X! " & ex.Message & vbCrLf &
                    ex.Sql)
                Catch ex As Exception
                    AppendToDebug("vb8EQXRHGFV6! " & ex.Message)
                End Try
            End If

            If pDT Is Nothing Then
                pDT = mProData.Execute(pSql)
            End If

            Return pDT

        End Function

        Private Function CopyCacheInvoiceFlavors(ByVal vFranchiseeOrgID As String,
        ByVal vFranchiseeDatabase As String, ByVal vFranchiseeFowstID As String,
        vFranchisorOrgID As String,
        ByVal vFranchisorDatabase As String, ByVal vFranchisorFowstID As String,
        ByVal vFranchiseGroupID As String,
        ByVal vFranchisorGroupName As String,
        ByVal vRegionalGroupID As String,
        ByVal vRegionName As String) As Integer

            Dim pDT As DataTable
            Dim pSW = Stopwatch.StartNew()
            Dim pOut As Integer

            Do
                LastMessage = "GetCopyableCacheInvoiceFlavors"
                pDT = GetCopyableCacheInvoiceFlavors(
                vFranchiseeDatabase:=vFranchiseeDatabase,
                vFranchiseeFowstID:=vFranchiseeFowstID,
                vFranchisorDatabase:=vFranchisorDatabase,
                vFranchiseGroupID:=vFranchiseGroupID,
                vGroupName:=vFranchisorGroupName,
                vRegionName:=vRegionName,
                vRegionID:=vRegionalGroupID)
                LastMessage = "GetCopyableCacheInvoiceFlavors count = " & pDT.Rows.Count
                AppendToLog("vb! GetCopyableCacheInvoiceFlavors " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
            "Rows = " & pDT.Rows.Count, mFolder)
                pOut += pDT.Rows.Count
                pSW.Restart()
                For Each pDR As DataRow In pDT.Rows
                    LastMessage = "Saving to CacheInvoiceFlavors " & (pDT.Rows.IndexOf(pDR) + 1) & " of " & pDT.Rows.Count
                    Dim pIU = Business.DataRowToInsertUpdate(pDR)
                    pIU.TableName = EscapeSqlName(vFranchisorDatabase) & ".dbo.CacheInvoiceFlavors"
                    If pDT.Columns.Contains("InvoiceNo") Then
                        pIU.Remove("InvoiceNo")
                    End If
                    pIU("FranchiseGroupID") = ReadyString(vFranchiseGroupID)
                    pIU("GroupName") = ReadyString(vFranchisorGroupName)
                    Dim pSql = pIU.GetUpdate("FowstID = " & ReadyString(pDR("FowstID")) & vbCrLf &
                    " AND InvoiceDate = " & ReadyString(CDate(pDR("InvoiceDate")).ToString(cGlobal.FormatDateSql)) & vbCrLf &
                    " AND ModifierProductID = " & ReadyString(pDR("ModifierProductID")) & vbCrLf &
                    " AND ModifierUnitMeasure = " & ReadyString(pDR("ModifierUnitMeasure")))
                    'AppendToLog("vb6Z5BG7QK56! " & pSql, , "FranchiseeDataExporter")

                    Dim pAffectedRecords = mProData.ExecuteSQL(pSql)
                    If pAffectedRecords = 0 Then
                        pSql = pIU.GetInsert()
                        mProData.ExecuteSQL(pSql)
                    End If
                Next
                AppendToLog("vb4ML7LN83W9! CopyCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                If pDT.Rows.Count >= mTop Then
                    AppendToLog("vb7PQFPHECG3! repeating copy - CopyCacheInvoiceDaily " & pSW.ElapsedMilliseconds.ToString(cGlobal.FormatInteger) & "ms" & vbCrLf &
                "Rows = " & pDT.Rows.Count, mFolder)
                Else
                    Exit Do
                End If
            Loop

            Return pOut

        End Function

        Public Shared Function DataRowToInsertUpdate(vDR As DataRow, Optional ByRef rIU As InsertUpdate = Nothing) As InsertUpdate

            Dim pDCs As DataColumnCollection
            Dim pValue As String

            If rIU Is Nothing Then
                rIU = New InsertUpdate()
            End If
            pDCs = vDR.Table.Columns

            For Each pDC As DataColumn In pDCs
                If pDC.ColumnName.StartsWith("_") Then
                    Continue For
                End If
                If pDC.ColumnName.StartsWith("Computed") Then
                    Continue For
                End If

                If vDR(pDC.ColumnName) Is DBNull.Value Then
                    pValue = "NULL"
                ElseIf pDC.DataType Is GetType(Boolean) Then
                    pValue = If(vDR(pDC.ColumnName), 1, 0)
                Else
                    Select Case MajorDataType(pDC.DataType)
                        Case eMajorDataType.Date, eMajorDataType.String
                            pValue = ReadyString(vDR(pDC.ColumnName))

                        Case eMajorDataType.Numeric
                            pValue = vDR(pDC.ColumnName)

                        Case Else
                            Continue For
                    End Select
                End If
                rIU(EscapeSqlName(pDC.ColumnName)) = pValue
            Next pDC

            Return rIU

        End Function

        Public Class Info
            Public StartDate As Date
            Public EndDate As Date
            Public DateOffset As Integer
            Public DetectInvoicesWithNoItems As Boolean
            Public FowstID As String
            Public Errors As New List(Of ErrorClass)

            Public Class ErrorClass
                Public FowstID As String ' FranchiseeOrgFowstID
                Public LogReference As String
                Public DateOffset As Integer
                Public ErrorMessage As String
            End Class
        End Class

        Public Shared Sub Test()

            Dim p As FranchiseeDataExporter

            p = New FranchiseeDataExporter(Common.WebRequest.ProData)
            p.CopyRecords(modClient.AbcOrgID)

        End Sub
    End Class
End Namespace
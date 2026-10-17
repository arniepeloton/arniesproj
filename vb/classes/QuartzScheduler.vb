Imports System.Collections.Specialized
Imports System.Net
Imports System.Security.Cryptography
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Quartz
Imports Quartz.Impl
Imports WebProj.WebProj.Data

Public Class QuartzScheduler

    Private Shared mInitialized As Boolean
    Private Shared mScheduler As IScheduler
    'Private Shared mEcommerceDataSync As EcommerceDataSync
    Private Shared mEcommerceDataExporter As EcommerceDataExporter

    Public Shared Async Sub Init()

        If mInitialized Then
            Return
        End If
        mInitialized = True

        'mEcommerceDataSync = New EcommerceDataSync()
        mEcommerceDataExporter = New EcommerceDataExporter()
        Dim pProps As NameValueCollection = New NameValueCollection()
        pProps.Add("quartz.serializer.type", "binary")
        Dim pFactory = New StdSchedulerFactory(pProps)
        mScheduler = Await pFactory.GetScheduler()
        Await mScheduler.Start()

        AddSleepKiller()
        'AddEcommerceDataSync()
        'AddEcommerceDataExporter()
        AddFranchiseeExporter()
        AddDaily()

    End Sub

    Private Shared Sub AddSleepKiller()

        Dim pJob As IJobDetail = JobBuilder.Create(Of SleepKiller)() _
            .WithIdentity("job1", "group1") _
            .Build()

        Dim pTrigger As ITrigger = TriggerBuilder.Create() _
            .WithIdentity("trigger1", "group1") _
            .WithSimpleSchedule(
                Sub(x)
                    x.WithIntervalInMinutes(1)
                    x.RepeatForever()
                End Sub) _
            .Build()

        mScheduler.ScheduleJob(pJob, pTrigger)

    End Sub

    'Private Shared Sub AddEcommerceDataSync()

    '    Dim pJob As IJobDetail = JobBuilder.Create(Of EcommerceDataSyncJob)() _
    '        .WithIdentity("EcommerceDataSyncJob", "group1") _
    '        .Build()

    '    Dim pTrigger As ITrigger = TriggerBuilder.Create() _
    '        .WithIdentity("EcommerceDataSyncTrigger", "group1") _
    '        .WithSimpleSchedule(
    '            Sub(x)
    '                x.WithIntervalInMinutes(1)
    '                x.RepeatForever()
    '            End Sub) _
    '        .Build()

    '    mScheduler.ScheduleJob(pJob, pTrigger)

    'End Sub

    Private Shared Sub AddEcommerceDataExporter()

        Dim pJob As IJobDetail = JobBuilder.Create(Of EcommerceDataExporterJob)() _
            .WithIdentity("EcommerceDataExporterJob", "group1") _
            .Build()

        Dim pTrigger As ITrigger = TriggerBuilder.Create() _
            .WithIdentity("EcommerceDataExporterTrigger", "group1") _
            .WithSimpleSchedule(
                Sub(x)
                    x.WithIntervalInMinutes(1)
                    x.RepeatForever()
                End Sub) _
            .Build()

        mScheduler.ScheduleJob(pJob, pTrigger)

    End Sub

    Private Shared Sub AddFranchiseeExporter()

        Dim pJob As IJobDetail = JobBuilder.Create(Of FranchiseeDataExporterJob)() _
            .WithIdentity("FranchiseeDataExporterJob", "group1") _
            .Build()

        Dim pTrigger As ITrigger = TriggerBuilder.Create() _
            .WithIdentity("FranchiseeDataExporterTrigger", "group1") _
            .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(1)) _
            .Build()
        mScheduler.ScheduleJob(pJob, pTrigger)

    End Sub

    Private Shared Sub AddDaily()

        Dim pJob As IJobDetail = JobBuilder.Create(Of DailyJob)() _
            .WithIdentity("DailyJob", "group1") _
            .Build()

        Dim pTrigger As ITrigger = TriggerBuilder.Create() _
            .WithIdentity("DailyJobTrigger", "group1") _
            .WithDailyTimeIntervalSchedule(
                Sub(x)
                    x.OnEveryDay()
                    'x.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(7, 0))
                    'x.EndingDailyAfterCount(1)
                End Sub) _
        .Build()

        mScheduler.ScheduleJob(pJob, pTrigger)

    End Sub

    Public Shared Sub TriggerEcommerceDataSyncJob()

        Dim pTrigger = mScheduler.GetTrigger(New TriggerKey("EcommerceDataSyncTrigger", "group1"))
        Try
            pTrigger.Start()
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

    End Sub

    Public Shared ReadOnly Property EcommerceDataExporter As EcommerceDataExporter
        Get

            Return mEcommerceDataExporter

        End Get
    End Property

    'Public Shared ReadOnly Property EcommerceDataSync As EcommerceDataSync
    '    Get

    '        Return mEcommerceDataSync

    '    End Get
    'End Property

    <DisallowConcurrentExecution>
    Public Class SleepKiller
        Implements IJob

        Dim mProData As ProData

        Private Function GetData() As DataTable

            Dim pDT As DataTable
            Dim pDR As DataRow
            Dim pSql As String

            pSql = "SET NOCOUNT ON" & vbCrLf &
                " /*" & vbCrLf &
                " Author:" & vbCrLf &
                " Benjamin Bolte" & vbCrLf &
                " brbolte@msn.com" & vbCrLf &
                " " & vbCrLf &
                " Purpose:" & vbCrLf &
                " Identify active or blocking connections, and list the active command on the connection." & vbCrLf &
                " */" & vbCrLf &
                " " & vbCrLf &
                " /*" & vbCrLf &
                " Status Definitions, per Books Online:" & vbCrLf &
                " " & vbCrLf &
                " Background	SPID is performing a background task. " & vbCrLf &
                " Sleeping	SPID is not currently executing. This usually indicates that the SPID is awaiting a command from the application. " & vbCrLf &
                " Runnable	SPID is currently executing. " & vbCrLf &
                " Dormant  	Same as Sleeping, except Dormant also indicates that the SPID has been reset after completing an RPC event. The reset cleans up resources used during the RPC event. This is a normal state and the SPID is available and waiting to execute further commands.  " & vbCrLf &
                " Rollback	The SPID is in rollback of a transaction. " & vbCrLf &
                " Defwakeup	Indicates that a SPID is waiting on a resource that is in the process of being freed. The waitresource field should indicate the resource in question. " & vbCrLf &
                " Spinloop	Process is waiting while attempting to acquire a spinlock used for concurrency control on SMP systems " & vbCrLf &
                " " & vbCrLf &
                " */" & vbCrLf &
                " create table #ProcCheck(" & vbCrLf &
                " 	Status varchar(50) ," & vbCrLf &
                " 	SPID int ," & vbCrLf &
                " 	CPU int ," & vbCrLf &
                " 	Pys_IO int ," & vbCrLf &
                " 	WaitTime int ," & vbCrLf &
                " 	BlockSPID int ," & vbCrLf &
                " 	LastCmd varchar(500) ," & vbCrLf &
                " 	HostName varchar(36) ," & vbCrLf &
                " 	ProgName varchar(100) ," & vbCrLf &
                " 	NTUser varchar(50) ," & vbCrLf &
                " 	LoginTime datetime ," & vbCrLf &
                " 	LastBatch datetime ," & vbCrLf &
                "     [Database] varchar(50)," & vbCrLf &
                " 	OpenTrans int)" & vbCrLf &
                " " & vbCrLf &
                " create table #ProcInfo(" & vbCrLf &
                " 	EventType varchar(100) ," & vbCrLf &
                " 	Parameters int ," & vbCrLf &
                " 	EventInfo varchar(7000)" & vbCrLf &
                " )" & vbCrLf &
                " " & vbCrLf &
                " INSERT INTO #ProcCheck(Status, SPID, CPU, Pys_IO, WaitTime, BlockSPID, HostName, ProgName, NTUSer, LoginTime, LastBatch, OpenTrans, [Database])" & vbCrLf &
                " SELECT status, SPID, CPU, Physical_IO, WaitTime, Blocked, SUBSTRING(HostName, 1, 36), SUBSTRING(Program_Name, 1, 100), SUBSTRING(nt_username, 1, 50), Login_Time, Last_Batch, Open_Tran" & vbCrLf &
                " ,case when p.dbid = 0" & vbCrLf &
                " 	then 'no database context'" & vbCrLf &
                "     else SUBSTRING(db_name(p.dbid), 1, 50)" & vbCrLf &
                "  END" & vbCrLf &
                " FROM master..sysprocesses p" & vbCrLf &
                " where (blocked > 0" & vbCrLf &
                " or spid in (select blocked from master..sysprocesses (NOLOCK) where blocked > 0)" & vbCrLf &
                " or open_tran > 0)" & vbCrLf &
                " and SPID <> @@SPID" & vbCrLf &
                " " & vbCrLf &
                " declare @spid int ," & vbCrLf &
                " 	@cmd varchar(7000)" & vbCrLf &
                " " & vbCrLf &
                " declare Procs cursor fast_forward for" & vbCrLf &
                " SELECT SPID FROM #ProcCheck" & vbCrLf &
                " " & vbCrLf &
                " OPEN Procs" & vbCrLf &
                " " & vbCrLf &
                " FETCH NEXT FROM Procs INTO @SPID" & vbCrLf &
                " WHILE @@FETCH_STATUS = 0" & vbCrLf &
                " 	BEGIN" & vbCrLf &
                " " & vbCrLf &
                " 	SET @cmd = 'DBCC INPUTBUFFER(' + CONVERT(varchar, @SPID) + ')'" & vbCrLf &
                " " & vbCrLf &
                " 	INSERT INTO #ProcInfo" & vbCrLf &
                " 	EXEC(@cmd)" & vbCrLf &
                " 	" & vbCrLf &
                " 	SELECT @cmd = EventInfo" & vbCrLf &
                " 	FROM #ProcInfo" & vbCrLf &
                " " & vbCrLf &
                " 	DELETE FROM #ProcInfo" & vbCrLf &
                " " & vbCrLf &
                " 	UPDATE #ProcCheck" & vbCrLf &
                " 	SET LastCmd = SUBSTRING(@cmd, 1, 500)" & vbCrLf &
                " 	WHERE SPID = @SPID" & vbCrLf &
                " " & vbCrLf &
                " 	FETCH NEXT FROM Procs INTO @SPID" & vbCrLf &
                " " & vbCrLf &
                " 	END" & vbCrLf &
                " " & vbCrLf &
                " CLOSE Procs" & vbCrLf &
                " DEALLOCATE Procs" & vbCrLf &
                " " & vbCrLf &
                " SELECT * FROM #ProcCheck	" & vbCrLf &
                " " & vbCrLf &
                " DROP TABLE #ProcCheck" & vbCrLf &
                " DROP TABLE #ProcInfo"

            pDT = mProData.Execute(pSql)

            pDT.Columns.Add("WaitTime2")
            For Each pDR In pDT.Rows
                pDR("WaitTime2") = FormatTime(CDec2(pDR("WaitTime")) \ 1000)
            Next pDR

            Return pDT

        End Function

        Public Function Execute(context As IJobExecutionContext) As Threading.Tasks.Task Implements IJob.Execute

            'AddToLog("Starting Execute SleepKiller. vbCJ3YYK6HVG!")
            Try
                Dim mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                Using pProData = mBusiness.ProData
                    mProData = pProData
                    Dim pDT = GetData()
                    For Each pDR As DataRow In pDT.Rows
                        If CStr2(pDR("Status")).Trim().ToLower() = "sleeping" Then
                            Dim pSeconds = (Now - CDate(pDR("LoginTime"))).TotalSeconds
                            pSeconds = Math.Abs(pSeconds)
                            If 600 < Math.Abs(pSeconds) Then
                                Dim pSql = "KILL " & pDR("SPID")
                                pProData.ExecuteSQL(pSql)
                                AddToLog("Successfully killed process." & vbCrLf &
                                    "Length of sleep: " & FormatDurationBySeconds(pSeconds) & vbCrLf &
                                    "Database: " & pDR("Database") & vbCrLf &
                                    "SQL: " & pDR("LastCmd") & vbCrLf &
                                    "ref: vbXBJLLVM5CA!")
                            End If
                        End If
                    Next
                End Using
            Catch ex As Exception
                AddToLog("An error occured: " & ex.Message & vbCrLf & GetStackTrace(False, ex))
            End Try

        End Function

        Private Sub AddToLog(vText As String)

            AppendToLog(vText, "temp\logs\QuartzScheduler", "QuartzScheduler")

        End Sub
    End Class

    '<DisallowConcurrentExecution>
    'Public Class EcommerceDataSyncJob
    '    Implements IJob

    '    Public Function Execute(context As IJobExecutionContext) As Threading.Tasks.Task Implements IJob.Execute

    '        AddToLog("Executing EcommerceDataSyncJob. ref: vbBD6TGS22XB!")
    '        mEcommerceDataSync.Execute()

    '    End Function

    '    Private Sub AddToLog(vText As String)

    '        AppendToLog(vText, "temp\logs\QuartzScheduler", "QuartzScheduler")

    '    End Sub
    'End Class

    <DisallowConcurrentExecution>
    Public Class EcommerceDataExporterJob
        Implements IJob

        Public Function Execute(context As IJobExecutionContext) As Threading.Tasks.Task Implements IJob.Execute

            'AddToLog("Executing EcommerceDataExporterJob. ref: vbYRFY4FMQTR!")
            mEcommerceDataExporter.Execute()

        End Function

        Private Sub AddToLog(vText As String)

            AppendToLog(vText, "temp\logs\QuartzScheduler", "QuartzScheduler")

        End Sub
    End Class

    <DisallowConcurrentExecution>
    Public Class FranchiseeDataExporterJob
        Implements IJob

        Public Function Execute(context As IJobExecutionContext) As Threading.Tasks.Task Implements IJob.Execute

            Try
                'ExecuteEveryFiveMinutes()
                'ExecuteEvery2Hours()
                If Now.Date = #2025/7/27# Then
                    'If Now.TimeOfDay > New TimeSpan(10, 0, 0) _
                    '    AndAlso Now.TimeOfDay < New TimeSpan(12 + 8, 0, 0) Then

                    '    Return Nothing
                    'End If
                Else
                    If Now.TimeOfDay > New TimeSpan(8, 0, 0) _
                        AndAlso Now.TimeOfDay < New TimeSpan(12 + 8, 0, 0) Then

                        Return Nothing
                    End If
                End If

                AddToLog("Executing FranchiseeExporterJob. ref: vbWU2HUJ5S22! ")
                'Cache() ' commented 2025_08_13__14_44_22_4138

                Dim pFilename = Common.ServerPath & "temp\ScheduledFranchiseeDataExporter.txt"
                Dim pFileText = FileText(pFilename)
                Dim pCacheDate = ParseDate(pFileText)
                If pCacheDate?.Date Is Nothing _
                    OrElse pCacheDate.Value.Date <> Now.Date Then

                    Const pOrgID = "892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba" ' MobizPotato_Corner_Franchise 
                    Using pProData = ProData.CreateFromConnectionInfo(
                    vServer:=Common.ConnectionInfo.Server,
                    vDatabase:=Common.ConnectionInfo.Database,
                    vUsername:=Common.ConnectionInfo.Username,
                    vPassword:=Common.ConnectionInfo.Password,
                    vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
                        Dim pBusiness = New Business(pProData)
                        Dim pDR = pBusiness.LibraryOrganization.ByID(pOrgID)
                        Dim pError As String

                        If pDR Is Nothing Then
                            pError = "Organization,  " & pOrgID & ", does not exist in LibraryOrganization. vbC6YVRVTU6U!"
                            AddToLog(pError)
                            Return Nothing
                        End If
                        If CStr2(pDR("Database")) = "" Then
                            pError = "Organization,  " & pOrgID & ", exist in LibraryOrganization but Database is null. vbKP2YUTNDRG!"
                            AddToLog(pError)
                            Return Nothing
                        End If

                        Dim pDatabase = pDR("Database")
                        pProData.ExecuteSQL("USE [" & EscapeSqlName(pDatabase) & "]")

                        Dim pFranchiseeDataExporter = New FranchiseeDataExporter(pProData) ' new

                        Try
                            AddToLog("vbJWUVE5K8Y4! before pFranchiseeDataExporter.CopyRecords(pOrgID)")
                            pProData.ExecuteSQL("USE [" & EscapeSqlName(pDatabase) & "]")
                            'pFranchiseeDataExporter.CopyRecords(pOrgID) ' commented 2025_08_13__14_44_22_4138
                        Catch ex As Exception
                            AddToLog("vbRX4VCW4Z29! Error in pFranchiseeDataExporter.CopyRecords(pOrgID): " & ex.Message)
                        End Try

                        ' 2025-08-01
                        'Try
                        '    AddToLog("vbYAJUZU6A2Q! before pBusiness.TransInvoice.CacheInvoicesModifiers()")
                        '    pProData.ExecuteSQL("USE [" & EscapeSqlName(pDatabase) & "]")
                        '    pBusiness.TransInvoice.CacheInvoicesModifiers()
                        '    AddToLog("vb8DSK7L5D67! after pBusiness.TransInvoice.CacheInvoicesModifiers()")
                        'Catch ex As Exception
                        '    AddToLog("vbLQVND574LD! Error in pBusiness.TransInvoice.CacheInvoicesModifiers(): " & ex.Message)
                        'End Try
                    End Using

                    CreateFolder(pFilename)
                    FileText(pFilename) = Now.ToString(cGlobal.FormatDateTimeSql)
                    Threading.Tasks.Task.Delay(1000 * 60 * 2)
                End If

            Catch ex As Exception
                AddToLog("vbXSAKC263TA! " & ex.Message)
                AppendToDebug("vbWR633KZEYY! " & ex.Message, True, ex, True)
            Finally
                AddToLog("vbGC88HKY3JT! finally FranchiseeDataExporterJob")
            End Try

        End Function

        Sub Cache()
            AddToLog("Executing Cache. ref: vb28Q8QTVHWD!")
            Dim pFilename = Common.ServerPath & "temp\ScheduledCaching.txt"
            Dim pFileText = FileText(pFilename)
            Dim pCacheDate = ParseDate(pFileText)
            If pCacheDate?.Date = Now.Date Then
                Return
            End If
            Const pOrgID = modClient.CincoOrgID
            'Const pOrgID = modClient.DemoCorpOrgID
            Using pProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:=Common.ConnectionInfo.Database,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
                Dim pBusiness = New Business(pProData)
                Dim pDR = pBusiness.LibraryOrganization.ByID(pOrgID)
                Dim pError As String

                If pDR Is Nothing Then
                    pError = "Organization,  " & pOrgID & ", does not exist in LibraryOrganization. vbV3KQG2QQ6X!"
                    AddToLog(pError)
                    Return
                End If
                If CStr2(pDR("Database")) = "" Then
                    pError = "Organization,  " & pOrgID & ", exist in LibraryOrganization but Database is null. vbCB5EQF9SKG!"
                    AddToLog(pError)
                    Return
                End If

                Dim pDatabase = pDR("Database")
                pProData.ExecuteSQL("USE [" & EscapeSqlName(pDatabase) & "]")
                Dim pDateFrom = Now.Date.AddDays(-3)
                Dim pDateTo = Now.Date.AddDays(-1)

                Try
                    Try
                        pBusiness.TransInvoice.CacheInvoiceDaily(vMaxAgeInDays:=999, pDateFrom, pDateTo, vFilterByDateCreated:=True)
                    Catch ex As Exception
                        AddToLog("vbRKKJCCHQLT! Error: " & ex.Message)
                    End Try

                    Try
                        pBusiness.TransInvoice.CacheInvoiceDaily(vMaxAgeInDays:=999, pDateFrom, pDateTo, vGroupByHour:=True,, vFilterByDateCreated:=True)
                    Catch ex As Exception
                        AddToLog("vb38JYF9SUSJ! Error: " & ex.Message)
                    End Try

                    Try
                        pBusiness.TransInvoice.CacheInvoiceFlavors(vMaxAgeInDays:=999, pDateFrom, pDateTo)
                    Catch ex As Exception
                        AddToLog("vbR8BYGW2Q7W! Error: " & ex.Message)
                    End Try

                    Try
                        pBusiness.TransInvoice.CacheDailySalesPerSKUReport(pDateFrom, pDateTo, pOrgID)
                    Catch ex As Exception
                        AddToLog("vbAGCYAHPY4P! Error: " & ex.Message)
                    End Try

                    Try
                        pBusiness.TransInvoice.CacheInvoiceComboMix(pDateFrom, pDateTo)
                    Catch ex As Exception
                        AddToLog("vbDXMQCKB4CU! Error: " & ex.Message)
                    End Try

                    Try
                        pBusiness.TransInvoice.CacheInvoiceFinishedProducts(vMaxAgeInDays:=999, pDateFrom, pDateTo)
                    Catch ex As Exception
                        AddToLog("vbGNNKJFGYJ6! Error: " & ex.Message)
                    End Try
                    'pBusiness.TransInvoice.CacheInvoiceProductSizes(vMaxAgeInDays:=1, pDateFrom, pDateTo)

                    Try
                        Dim pMonthlyDateFrom = DateRange.GetMonth(pDateFrom.AddMonths(-1)).DateFrom
                        pBusiness.TransInvoice.CacheInvoiceDaily(vMaxAgeInDays:=999, pMonthlyDateFrom, pDateTo, vGroupByMonth:=True, vFilterByDateCreated:=True)
                    Catch ex As Exception
                        AddToLog("vb2ZSAAJKBW6! Error: " & ex.Message)
                    End Try

                    CreateFolder(pFilename)
                    FileText(pFilename) = Now.ToString(cGlobal.FormatDateTimeSql)
                    AddToLog("vbURJWASX5QT! after caches")
                    Return
                Catch ex As Exception
                    AddToLog("vbT6U8UFJ3YU! cache error: " & ex.Message)
                    AppendToDebug("vbEHP6PSTVUC! " & ex.Message, True, ex, True)
                Finally
                End Try
            End Using

        End Sub

        Async Sub ExecuteEveryFiveMinutes()

            Const pIntervalInMinutes As Integer = 30
            Try
                Await Task.Delay(2) ' i dont know if this is needed for async
                Dim pFilename = IO.Path.Combine(cGlobal.AppPath, "temp\ExecuteEveryFiveMinutes.txt")
                Dim pFileText = FileText(pFilename)
                Dim pDate = ParseDate(pFileText)
                If pDate.HasValue _
                    AndAlso Math.Abs((pDate.Value - Now).TotalMinutes) < pIntervalInMinutes Then

                    Exit Sub
                End If
                FileText(pFilename) = Now.ToString(cGlobal.FormatDateTimeSql)
                Call New WebClient().DownloadString("http://localhost:8083/ThirdPartyPos/CallImportOrganization")
                Call New WebClient().DownloadString("http://localhost:8083/ThirdPartyPos/DailyRun")
            Catch ex As Exception
                AddToLog("vb3KPEKKJHRN! Error in ExecuteEveryFiveMinutes: " & ex.Message)
            End Try

        End Sub

        Sub ExecuteEvery2Hours()

            Const pIntervalInMinutes As Integer = 2 * 60
#If DEBUG Then
            Const pOrgID = modClient.DemoCorpOrgID
#Else
                Const pOrgID = modClient.CincoOrgID
#End If

            Try
                Dim pFilename = IO.Path.Combine(cGlobal.AppPath, "temp\ExecuteEvery2Hours.txt")
                Dim pFileText = FileText(pFilename)
                Dim pDate = ParseDate(pFileText)
                If pDate.HasValue _
                    AndAlso Math.Abs((pDate.Value - Now).TotalMinutes) < pIntervalInMinutes Then

                    Exit Sub
                End If
                FileText(pFilename) = Now.ToString(cGlobal.FormatDateTimeSql)

                Using pProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:=Common.ConnectionInfo.Database,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
                    Dim pBusiness = New Business(pProData)
                    Dim pDR = pBusiness.LibraryOrganization.ByID(pOrgID)
                    Dim pError As String

                    If pDR Is Nothing Then
                        pError = "Organization,  " & pOrgID & ", does not exist in LibraryOrganization. vbV3KQG2QQ6X!"
                        AddToLog(pError)
                        Return
                    End If
                    If CStr2(pDR("Database")) = "" Then
                        pError = "Organization,  " & pOrgID & ", exist in LibraryOrganization but Database is null. vbCB5EQF9SKG!"
                        AddToLog(pError)
                        Return
                    End If

                    Dim pDatabase = pDR("Database")
                    pProData.CommandTimeout = 365 * 60 * 60
                    pProData.ExecuteSQL("USE [" & EscapeSqlName(pDatabase) & "]")

                    Dim pSql = " UPDATE cacheinvoices" & vbCrLf &
                        " set FromFranchisee = tblDerived.IsFranchisee" & vbCrLf &
                        " FROM (" & vbCrLf &
                        " select  top 50000 cacheinvoices.id" & vbCrLf &
                        " ,cacheinvoices.fowstid" & vbCrLf &
                        " ,MobizLibraryOrganizationFowst.name AS FowstName" & vbCrLf &
                        " ,MobizLibraryOrganization.CompanyName" & vbCrLf &
                        " ,case when MobizLibraryOrganizationFowst.organizationid = 'd362ffdb-15d1-451f-97dc-c567b94fb410' then 0 else 1 end as IsFranchisee" & vbCrLf &
                        "  from cacheinvoices" & vbCrLf &
                        " JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = cacheinvoices.fowstid" & vbCrLf &
                        " JOIN MobizLibraryOrganization ON MobizLibraryOrganization.ID = MobizLibraryOrganizationFowst.OrganizationID" & vbCrLf &
                        " where cacheinvoices.FromFranchisee is null" & vbCrLf &
                        " and year(cacheinvoices.invoicedate) >= 2025" & vbCrLf &
                        " ) tblDerived" & vbCrLf &
                        " JOIN cacheInvoices ON cacheInvoices.id = tblDerived.ID"
                    Dim pResult = pProData.ExecuteSQL(pSql)
                    AddToLog("vbDYDTYVV4AD! No. of updated records: " & pResult)
                End Using
            Catch ex As Exception
                AddToLog("vb3KPEKKJHRN! Error in ExecuteEvery2Hours: " & ex.Message)
            End Try

        End Sub

        Private Sub AddToLog(vText As String)

            AppendToLog(vText, "temp\logs\QuartzScheduler", "QuartzScheduler")

        End Sub
    End Class

    <DisallowConcurrentExecution>
    Public Class DailyJob
        Implements IJob

        Public Function Execute(context As IJobExecutionContext) As Threading.Tasks.Task Implements IJob.Execute

            Try
                Dim pFilename = Common.ServerPath & "temp\DailySchedule.txt"
                Dim pFileText = FileText(pFilename)
                Dim pCacheDate = ParseDate(pFileText)
                If pCacheDate?.Date Is Nothing _
                    OrElse pCacheDate.Value.Date <> Now.Date Then

                    AddToLog("Executing DailyJob. ref: vbM79EEXNC7T!")
                    Const pOrgID = modClient.CincoOrgID
                    Using pProData = ProData.CreateFromConnectionInfo(
                    vServer:=Common.ConnectionInfo.Server,
                    vDatabase:=Common.ConnectionInfo.Database,
                    vUsername:=Common.ConnectionInfo.Username,
                    vPassword:=Common.ConnectionInfo.Password,
                    vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)
                        Dim pBusiness = New Business(pProData)
                        Dim pDR = pBusiness.LibraryOrganization.ByID(pOrgID)
                        Dim pError As String

                        If pDR Is Nothing Then
                            pError = "Organization,  " & pOrgID & ", does not exist in LibraryOrganization. vbKUH8SFFPYQ!"
                            AddToLog(pError)
                            Return Nothing
                        End If
                        If CStr2(pDR("Database")) = "" Then
                            pError = "Organization,  " & pOrgID & ", exist in LibraryOrganization but Database is null. vbMYNBMXJ4PX!"
                            AddToLog(pError)
                            Return Nothing
                        End If

                        Dim pDatabase = pDR("Database")
                        pProData.ExecuteSQL("USE [" & EscapeSqlName(pDatabase) & "]")

                        'FunctionToRun(pProData)
                        'RunDeviceMonitoringCheckProd()
                        AddToLog("DailyJob executed. ref: vbUVEL6UTTV9!")
                    End Using

                    CreateFolder(pFilename)
                    FileText(pFilename) = Now.ToString(cGlobal.FormatDateTimeSql)
                End If

            Catch ex As Exception
                AddToLog("vbPZAN267VQ9! Error in DailyJob: " & ex.Message)
                AppendToDebug("vbB2LVB9UHJS! Error in DailyJob: " & ex.Message, True, ex, True)
            Finally
                'AddToLog("vbK7Z64LJWQX! finally DailyJob") ' commented 2025-07-23
            End Try

        End Function

        Sub FunctionToRun(vProData As ProData)

            Dim pSql = " update CacheInvoices" & vbCrLf &
" set FranchiseGroupID = (Select distinct LibraryFranchiseGroup.ID from LibraryFranchiseGroup" & vbCrLf &
" INNER JOIN MobizLibraryOrganizationFranchisee on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
" INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
" WHERE MobizLibraryorganizationFOWST.ID= CacheInvoices.FowstID)" & vbCrLf &
" where FranchiseGroupID is null;" & vbCrLf &
" update CacheInvoices" & vbCrLf &
" set GroupName = (Select distinct LibraryFranchiseGroup.GroupName from LibraryFranchiseGroup" & vbCrLf &
" INNER JOIN MobizLibraryOrganizationFranchisee on LibraryFranchiseGroup.ID = MobizLibraryOrganizationFranchisee.FranchiseGroupID " & vbCrLf &
" INNER JOIN MobizLibraryOrganizationFOWST on MobizLibraryOrganizationFranchisee.FranchiseeOrgFowstID = MobizLibraryOrganizationFOWST.ID " & vbCrLf &
" WHERE MobizLibraryorganizationFOWST.ID= CacheInvoices.FowstID)" & vbCrLf &
" where GroupName is null"
            vProData.Execute(pSql)
        End Sub

        Function RunDeviceMonitoringCheckProd() As String
            Dim triggers As Triggers

            Using pProData = ProData.CreateFromConnectionInfo(
                vServer:=Common.ConnectionInfo.Server,
                vDatabase:=Common.ConnectionInfo.Database,
                vUsername:=Common.ConnectionInfo.Username,
                vPassword:=Common.ConnectionInfo.Password,
                vIntegratedSecurity:=Common.ConnectionInfo.IntegratedSecurity)

                pProData.FailedConnectRetryDuration = 60 * 1000

                triggers = New Triggers(pProData)

                triggers.ConnectToDatabase("MobizCinco_Corporation")

                triggers.RunDeviceMonitoringByManager("MobizCinco_Corporation")

                triggers.EmailAMAndDMforDeviceMonitoring("MobizCinco_Corporation")

                Return CStr(triggers.RunDeviceMonitoringCheck("MobizCinco_Corporation"))
            End Using

        End Function

        Private Sub AddToLog(vText As String)

            AppendToLog(vText, "temp\logs\QuartzScheduler", "DailySchedule")

        End Sub
    End Class

End Class


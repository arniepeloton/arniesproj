Namespace WebProj

    Public Class TriggersController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Triggers

        Dim mBusiness As Business
        Private sFtp As Renci.SshNet.SftpClient
        Private myFileStream As System.IO.FileStream

        Function Index() As ActionResult
            StartTrigger()
            AppendToLog("Reports and Subscription has been triggered successfully")
            Return View()
        End Function

        Function StartTrigger()
            Dim dtSubscriptionDatabase As DataTable

            dtSubscriptionDatabase = GetAllSubscriptionDatabase()

            For i As Integer = 0 To dtSubscriptionDatabase.Rows.Count - 1
                If (CStr(dtSubscriptionDatabase.Rows(i).Item("Database")).Contains("Nike")) Then
                    mBusiness = New Business(ProData.CreateFromConnectionInfo(Common.ConnectionInfo))
                    mBusiness.ProData.Execute("USE " & EscapeSqlName(CStr(dtSubscriptionDatabase.Rows(i).Item("Database"))))
                    NikeSEAReport(dtSubscriptionDatabase.Rows(i).Item("Database"))
                    ShopperTrakReport(dtSubscriptionDatabase.Rows(i).Item("Database"))
                    Exit For
                End If
            Next

            '''Trigger for Subscription
            CheckForSubscriber()

        End Function

        Function CheckForSubscriber()

            TimedThread(
                Sub()

                    Try

                        Dim clsSubscription As New SubscriptionController
                        clsSubscription.DownloadNewSubscriber()

                    Catch ex As Exception
                        AppendToLog(ex.Message)
                    Finally
                    End Try

                End Sub
            )
        End Function

        Sub ShopperTrakReport(ByVal CompanyDBName As String)
            Dim pSql As String
            Dim dtSchedule As DataTable
            Dim dtPrevSchedule As DataTable
            Dim fileEntry As String
            Dim m_Day As Integer

            Dim t As New ShopperTrakController
            Dim SalesDataFilePath As String
            Dim LaborDataFilePath As String

            If (CompanyDBName.Contains("Nike")) Then
                'check if new sched should be applied

                m_Day = Today.DayOfWeek

                If m_Day = 1 Then
                    pSql = "Select top 1 * from LibraryOrganizationWorkScheduleEmployee where convert(date,EffectivityDateFrom) = " & ReadyString(Today)

                    dtSchedule = mBusiness.ProData.Execute(pSql)

                    If dtSchedule.Rows.Count > 0 Then 'new sched found
                        'get last schedule and create csv file

                        Dim pFinder As Finder

                        pFinder = New Finder

                        With pFinder
                            .ProData = Common.WebRequest.ProData

                            .Distinct = True

                            With .Fields
                                With .Add("Account_ID", SqlDbType.UniqueIdentifier)
                                    .Searchable = False
                                    .Expression = "dbo.MobizLibraryOrganizationFOWST.ShopperTrakAccountID"
                                End With
                                .Add("PeopleNo", SqlDbType.Int)
                                .Add("MondayFrom", SqlDbType.DateTime2)
                                .Add("MondayTo", SqlDbType.DateTime2)
                                .Add("MondayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("MondayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("MondayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("MondayDinnerBreakTo", SqlDbType.DateTime2)
                                .Add("TuesdayFrom", SqlDbType.DateTime2)
                                .Add("TuesdayTo", SqlDbType.DateTime2)
                                .Add("TuesdayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("TuesdayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("TuesdayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("TuesdayDinnerBreakTo", SqlDbType.DateTime2)
                                .Add("WednesdayFrom", SqlDbType.DateTime2)
                                .Add("WednesdayTo", SqlDbType.DateTime2)
                                .Add("WednesdayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("WednesdayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("WednesdayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("WednesdayDinnerBreakTo", SqlDbType.DateTime2)
                                .Add("ThursdayFrom", SqlDbType.DateTime2)
                                .Add("ThursdayTo", SqlDbType.DateTime2)
                                .Add("ThursdayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("ThursdayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("ThursdayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("ThursdayDinnerBreakTo", SqlDbType.DateTime2)
                                .Add("FridayFrom", SqlDbType.DateTime2)
                                .Add("FridayTo", SqlDbType.DateTime2)
                                .Add("FridayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("FridayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("FridayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("FridayDinnerBreakTo", SqlDbType.DateTime2)
                                .Add("SaturdayFrom", SqlDbType.DateTime2)
                                .Add("SaturdayTo", SqlDbType.DateTime2)
                                .Add("SaturdayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("SaturdayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("SaturdayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("SaturdayDinnerBreakTo", SqlDbType.DateTime2)
                                .Add("SundayFrom", SqlDbType.DateTime2)
                                .Add("SundayTo", SqlDbType.DateTime2)
                                .Add("SundayLunchBreakFrom", SqlDbType.DateTime2)
                                .Add("SundayLunchBreakTo", SqlDbType.DateTime2)
                                .Add("SundayDinnerBreakFrom", SqlDbType.DateTime2)
                                .Add("SundayDinnerBreakTo", SqlDbType.DateTime2)
                            End With

                            .SqlFrom = "FROM dbo.LibraryOrganizationWorkSchedule INNER JOIN " & vbCrLf &
                                  "dbo.LibraryOrganizationWorkScheduleEmployee ON  " & vbCrLf &
                                  "dbo.LibraryOrganizationWorkSchedule.ID = dbo.LibraryOrganizationWorkScheduleEmployee.WorkScheduleID INNER JOIN " & vbCrLf &
                                  "dbo.MobizLibraryOrganizationFOWST ON dbo.LibraryOrganizationWorkScheduleEmployee.FowstID = dbo.MobizLibraryOrganizationFOWST.ID INNER JOIN " & vbCrLf &
                                  "dbo.LibraryOrganizationEmployee ON dbo.LibraryOrganizationWorkScheduleEmployee.PeopleID = dbo.LibraryOrganizationEmployee.ID"

                            .SqlWhere = "WHERE IsIncludeToShopperTrakReport = 1  AND convert(date,EffectivityDateFrom) = " & ReadyString(DateAdd(DateInterval.Day, -7, Today)) & " AND convert(date,EffectivityDateTo) = " & ReadyString(DateAdd(DateInterval.Day, -1, Today))

                            dtPrevSchedule = mBusiness.ProData.Execute(.CreateSql)
                        End With

                        Dim CurrentDate As Date = CDate(DateAdd(DateInterval.Day, -8, Today))
                        Dim i As Integer
                        Dim sb As StringBuilder = New StringBuilder()
                        Dim m_DayOfWeek As String

                        While DateDiff(DateInterval.Day, CurrentDate, CDate(DateAdd(DateInterval.Day, -2, Today))) >= 0

                            Select Case CInt(DateTime.Parse(CurrentDate).DayOfWeek)
                                Case 0
                                    m_DayOfWeek = "Sunday"
                                Case 1
                                    m_DayOfWeek = "Monday"
                                Case 2
                                    m_DayOfWeek = "Tuesday"
                                Case 3
                                    m_DayOfWeek = "Wednesday"
                                Case 4
                                    m_DayOfWeek = "Thursday"
                                Case 5
                                    m_DayOfWeek = "Friday"
                                Case 6
                                    m_DayOfWeek = "Saturday"
                            End Select

                            dtPrevSchedule.DefaultView.RowFilter = m_DayOfWeek & "FROM IS NOT NULL "

                            Dim dt As DataTable = dtPrevSchedule.DefaultView.ToTable
                            Dim j As Integer = 0

                            If dt.Rows.Count > 0 Then
                                For j = 0 To dt.Rows.Count - 1
                                    If IsDate(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakFrom")) Then
                                        sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "FROM"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakFrom"), "HHmmss"))
                                        sb.AppendLine()
                                        sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "LunchBreakTo"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "To"), "HHmmss"))
                                    End If
                                    If IsDate(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakFrom")) Then
                                        sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "FROM"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakFrom"), "HHmmss"))
                                        sb.AppendLine()
                                        sb.Append(dt.Rows(j).Item("Account_ID") & "," & Format(CurrentDate, "yyyyMMdd") & "," & dt.Rows(j).Item("PeopleNo") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "DinnerBreakTo"), "HHmmss") & "," & Format(dt.Rows(j).Item(m_DayOfWeek & "To"), "HHmmss"))
                                    End If
                                    sb.AppendLine()
                                Next
                            End If
                            CurrentDate = DateAdd(DateInterval.Day, 1, CurrentDate)
                        End While

                        FileText(Common.ServerPath & "temp\LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt") = sb.ToString

                        LaborDataFilePath = "LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt"

                        'sFtp = New Renci.SshNet.SftpClient("data.shoppertrak.com", "athletic", "waxQ85Whisp")
                        'sFtp.Connect()

                        'fileEntry = Common.ServerPath & "temp\LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -1, Today)), "yyyyMMdd") & ".txt"
                        'sFtp.ChangeDirectory("Labor")

                        'Try
                        '    myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                        '    sFtp.UploadFile(myFileStream, fileEntry, True)
                        '    myFileStream.Close()

                        'Catch ex As Exception
                        '    AppendToDebug(ex.Message)
                        'End Try

#If DEBUG Then
                        sFtp = New Renci.SshNet.SftpClient("192.168.1.80", "Administrator", "secret")
                        sFtp.Connect()
                        fileEntry = Common.ServerPath & "temp\" & LaborDataFilePath
                        sFtp.ChangeDirectory("LABOR")

                        Try
                            myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                            sFtp.UploadFile(myFileStream, LaborDataFilePath, True)
                            myFileStream.Close()

                            SendMail("customercare@mobiz-apps.com",
                                    "francis.hernandez@mobiz-apps.com",
                                    "Mobiz",
                                    "LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                    "ShopperTrak Labor Data File",
                                    "smtp.live.com",
                                    587,
                                    "customercare@mobiz-apps.com",
                                    "m0b!zp30pl30nly!", , ,
                                    "Labor_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                    fileEntry,
                                    True)

                        Catch ex As Exception
                            AppendToDebug(ex.Message)
                        End Try

                        
#Else
                    sFtp = New Renci.SshNet.SftpClient("data.shoppertrak.com", "athletic", "waxQ85Whisp")
                        sFtp.Connect()
                        fileEntry = Common.ServerPath & "temp\" & LaborDataFilePath
                        sFtp.ChangeDirectory("LABOR")

                        Try
                            myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                            sFtp.UploadFile(myFileStream, LaborDataFilePath, True)
                            myFileStream.Close()

                            SendMail("customercare@mobiz-apps.com",
                                    "francis.hernandez@mobiz-apps.com",
                                    "Mobiz",
                                    "LABOR_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                    "ShopperTrak Labor Data File",
                                    "smtp.live.com",
                                    587,
                                    "customercare@mobiz-apps.com",
                                    "m0b!zp30pl30nly!", , ,
                                    "Labor_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                    fileEntry,
                                    True)

                        Catch ex As Exception
                            AppendToDebug(ex.Message)
                        End Try
#End If

                        '"christinegrace.perez@yahoo.com",

                    End If



                    SalesDataFilePath = ShopperTrakSalesDataReport(DateAdd(DateInterval.Day, -8, Today), DateAdd(DateInterval.Day, -2, Today))

                    If Not SalesDataFilePath Is Nothing Then
#If DEBUG Then
                        sFtp = New Renci.SshNet.SftpClient("192.168.1.80", "Administrator", "secret")
                        sFtp.Connect()

                        fileEntry = Common.ServerPath & "temp\" & SalesDataFilePath
                        sFtp.ChangeDirectory("HourlySummary")

                        Try
                            myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                            sFtp.UploadFile(myFileStream, SalesDataFilePath, True)
                            myFileStream.Close()

                            SendMail("customercare@mobiz-apps.com",
                                    "francis.hernandez@mobiz-apps.com",
                                    "Mobiz",
                                    "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                    "ShopperTrak Sales Data File",
                                    "smtp.live.com",
                                    587,
                                    "customercare@mobiz-apps.com",
                                    "m0b!zp30pl30nly!", , ,
                                    "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                    fileEntry,
                                    True)

                        Catch ex As Exception
                            AppendToDebug(ex.Message)
                        End Try
#Else
                    sFtp = New Renci.SshNet.SftpClient("data.shoppertrak.com", "athletic", "waxQ85Whisp")
                    sFtp.Connect()

                    fileEntry = Common.ServerPath & "temp\" & SalesDataFilePath
                    sFtp.ChangeDirectory("HourlySummary")

                    Try
                        myFileStream = New System.IO.FileStream(fileEntry, System.IO.FileMode.Open, System.IO.FileAccess.Read)

                        sFtp.UploadFile(myFileStream, SalesDataFilePath, True)
                        myFileStream.Close()

                    Catch ex As Exception
                        AppendToDebug(ex.Message)
                    End Try

                    SendMail("customercare@mobiz-apps.com",
                                                     "francis.hernandez@mobiz-apps.com",
                                                     "Mobiz",
                                                     "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd"),
                                                     "ShopperTrak Labor Data File",
                                                     "smtp.live.com",
                                                     587,
                                                     "customercare@mobiz-apps.com",
                                                     "m0b!zp30pl30nly!", , ,
                                                     "SALES_" & Format(CDate(DateAdd(DateInterval.Day, -2, Today)), "yyyyMMdd") & ".txt",
                                                     fileEntry,
                                                     True)
#End If
                    End If


                    

                End If
            End If
        End Sub

        <HttpPost()>
        Function ShopperTrakSalesDataReport(ByVal DateFrom As String, ByVal DateTo As String) As String
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String
            Dim Fowst As String()
            Dim CtrFowst As Integer = 1


            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            If Not System.IO.File.Exists(Common.ServerPath & "temp\SALES_" & Format(CDate(DateTo), "yyyyMMdd") & ".txt") Then

                pFinder = New Finder

                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        With .Add("Account_ID", SqlDbType.UniqueIdentifier)
                            .Searchable = False
                            .Expression = "dbo.MobizLibraryOrganizationFOWST.ShopperTrakAccountID"
                        End With


                        With .Add("InvoiceDate", SqlDbType.UniqueIdentifier)
                            .Searchable = False
                            .Expression = "replace(CONVERT(VARCHAR(10),dbo.TransInvoice.InvoiceDate,21),'-','')"
                        End With

                        With .Add("FullInvoiceDate", SqlDbType.UniqueIdentifier)
                            .Searchable = False
                            .Expression = "dbo.TransInvoice.InvoiceDate"
                        End With

                        With .Add("Sales", SqlDbType.VarChar)
                            .Expression = "dbo.TransInvoice.TotalAmount - dbo.TransInvoice.DiscountAmount"
                        End With

                        With .Add("Category", SqlDbType.VarChar)
                            .Expression = "''"
                        End With

                        With .Add("Currency", SqlDbType.VarChar)
                            .Expression = "'PHP'"
                        End With

                    End With

                    .SqlFrom = "FROM dbo.TransInvoice INNER JOIN " &
                                "dbo.MobizLibraryOrganizationFOWST ON dbo.TransInvoice.FowstID = dbo.MobizLibraryOrganizationFOWST.ID "

                    .SqlWhere = "WHERE dbo.TransInvoice.InvoiceNumber > 0 AND " &
                                "CONVERT(date, dbo.TransInvoice.InvoiceDate) BETWEEN " & ReadyString(DateFrom) & " AND " &
                                ReadyString(DateTo) & " AND IsIncludeToShopperTrakReport = 1"

                    .SqlOrder = "ORDER BY dbo.TransInvoice.InvoiceDate"

                    pDT = mBusiness.ProData.Execute(.CreateSql)

                    Dim DaysApart As Integer

                    DaysApart = DateDiff(DateInterval.Day, CDate(DateFrom), CDate(DateTo))

                    Dim CurrentDate As Date = CDate(DateFrom)
                    Dim i As Integer
                    Dim sb As StringBuilder = New StringBuilder()

                    While DateDiff(DateInterval.Day, CurrentDate, CDate(DateTo)) >= 0
                        i = 0
                        While i < 23
                            pDT.DefaultView.RowFilter = "FullInvoiceDate >= '" & Format(CurrentDate, "yyyy-MM-dd " & i & ":00:01") & "' AND FullInvoiceDate <= '" & Format(CurrentDate, "yyyy-MM-dd " & i + 1 & ":00:00") & "'"
                            Dim dt As DataTable = pDT.DefaultView.ToTable
                            Dim j As Integer = 0
                            Dim TotalSales As Double = 0.0

                            If dt.Rows.Count > 0 Then
                                While j < dt.Rows.Count
                                    TotalSales = TotalSales + dt.Rows(j).Item("Sales")
                                    j += 1
                                End While
                                sb.Append(dt.Rows(0).Item("Account_ID") & "," & dt.Rows(0).Item("InvoiceDate") & "," & Format(i + 1, "##") & "0000," & Format(TotalSales, "#.00") & "," & dt.Rows.Count & ",,PHP")
                                sb.AppendLine()
                            End If
                            i += 1
                        End While

                        CurrentDate = DateAdd(DateInterval.Day, 1, CurrentDate)
                    End While

                    FileText(Common.ServerPath & "temp\SALES_" & Format(CDate(DateTo), "yyyyMMdd") & ".txt") = sb.ToString

                    Return "SALES_" & Format(CDate(DateTo), "yyyyMMdd") & ".txt"
                End With
            End If
        End Function

        Sub NikeSEAReport(ByVal CompanyDBName As String)
            Dim m_Day As Integer

            Dim DateFrom As Date = CDate(DateAdd(DateInterval.Day, -8, Today))
            Dim DateTo As Date = CDate(DateAdd(DateInterval.Day, -2, Today))
            Dim clsReportToPrincipal As New ReportToPrincipal
            Dim m_Path As String

            If (CompanyDBName.Contains("Nike")) Then

                m_Day = Today.DayOfWeek

                If m_Day = 1 Then

                    m_Path = "InvBal_Week_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    If Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path) Then
                        If GetWeeklyInventoryReport(DateTo) Then

#If DEBUG Then
                            SendMail("customercare@mobiz-apps.com",
                                                             "francis.hernandez@mobiz-apps.com",
                                                             "Mobiz",
                                                             Replace(m_Path, ".csv", ""),
                                                             Replace(m_Path, ".csv", ""),
                                                             "smtp.live.com",
                                                             587,
                                                             "customercare@mobiz-apps.com",
                                                             "m0b!zp30pl30nly!", , ,
                                                             m_Path,
                                                             Common.ServerPath & "temp\" & m_Path,
                                                             True)
#Else
                            SendMail("customercare@mobiz-apps.com",
                                                             "sellthru.sea@nike.com, christinegrace.perez@yahoo.com, francis.hernandez@mobiz-apps.com",
                                                             "Mobiz",
                                                             Replace(m_Path, ".csv", ""),
                                                             Replace(m_Path, ".csv", ""),
                                                             "smtp.live.com",
                                                             587,
                                                             "customercare@mobiz-apps.com",
                                                             "m0b!zp30pl30nly!", , ,
                                                             m_Path,
                                                             Common.ServerPath & "temp\" & m_Path,
                                                             True)
#End If

                        End If
                    End If

                    m_Path = "POS_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    If Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path) Then
                        If (GetWeeklySalesReport(DateFrom, DateTo)) Then
#If DEBUG Then
                            SendMail("customercare@mobiz-apps.com",
                             "francis.hernandez@mobiz-apps.com",
                             "Mobiz",
                             Replace(m_Path, ".csv", ""),
                             Replace(m_Path, ".csv", ""),
                             "smtp.live.com",
                             587,
                             "customercare@mobiz-apps.com",
                             "m0b!zp30pl30nly!", , ,
                             m_Path,
                             Common.ServerPath & "temp\" & m_Path,
                             True)
#Else
                            SendMail("customercare@mobiz-apps.com",
                             "sellthru.sea@nike.com, christinegrace.perez@yahoo.com, francis.hernandez@mobiz-apps.com",
                             "Mobiz",
                             Replace(m_Path, ".csv", ""),
                             Replace(m_Path, ".csv", ""),
                             "smtp.live.com",
                             587,
                             "customercare@mobiz-apps.com",
                             "m0b!zp30pl30nly!", , ,
                             m_Path,
                             Common.ServerPath & "temp\" & m_Path,
                             True)

#End If

                        End If
                    End If
                End If

                Dim m_StartOfDay As Date = Today.AddDays(-4)

                If Today.Day = 5 Then
                    m_Path = "InvMvmt_Month_" & Today.AddDays(-6).Year & Format(Today.AddDays(-6).Month, "00") & Format(Today.AddDays(-5).Day, "00") & ".csv"

                    'm_Path = GetMonthInventoryReport()
                    'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

                    If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then
                        If GetMonthInventoryReport(m_StartOfDay.AddMonths(-1), Today.AddDays(-5)) Then
#If DEBUG Then
                            SendMail("customercare@mobiz-apps.com",
                               "francis.hernandez@mobiz-apps.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "customercare@mobiz-apps.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#Else
                            SendMail("customercare@mobiz-apps.com",
                             "sellthru.sea@nike.com, christinegrace.perez@yahoo.com, francis.hernandez@mobiz-apps.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "customercare@mobiz-apps.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#End If

                        End If
                    End If
                End If

                If Today.Day = 1 Then
                    DateTo = CDate(DateAdd(DateInterval.Day, -1, Today))
                    m_Path = "InvBal_Asof_" & CDate(DateTo).Year & Format(CDate(DateTo).Month, "00") & Format(CDate(DateTo).Day, "00") & ".csv"

                    'm_Path = GetMonthInventoryReport()
                    'sellthru.sea@nike.com, christinegrace.perez@yahoo.com, 

                    If (Not System.IO.File.Exists(Common.ServerPath & "temp\" & m_Path)) Then
                        If GetWeeklyInventoryReport(CDate(DateAdd(DateInterval.Day, -1, Today))) Then
#If DEBUG Then
                            SendMail("customercare@mobiz-apps.com",
                               "francis.hernandez@mobiz-apps.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "customercare@mobiz-apps.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#Else
                            SendMail("christinegrace.perez@yahoo.com", "francis.hernandez@mobiz-apps.com",
                               "Mobiz",
                               Replace(m_Path, ".csv", ""),
                               Replace(m_Path, ".csv", ""),
                               "smtp.live.com",
                               587,
                               "customercare@mobiz-apps.com",
                               "m0b!zp30pl30nly!", , ,
                               m_Path,
                               Common.ServerPath & "temp\" & m_Path,
                               True)
#End If

                        End If
                    End If
                End If

            End If
        End Sub

        Function GetWeeklyInventoryReport(ByVal m_date As Date) As Boolean
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData

                .Distinct = True
                With .Fields
                    With .Add("Account_ID", SqlDbType.VarChar)
                        .Expression = "t.Account_ID"
                    End With
                    With .Add("Store_ID", SqlDbType.VarChar)
                        .Expression = "t.Store_ID"
                    End With
                    With .Add("Inv_Balance_Date", SqlDbType.VarChar)
                        .Expression = "t.Inv_Balance_Date"
                    End With
                    With .Add("Product_Code", SqlDbType.Date)
                        .Expression = "t.Product_Code"
                    End With
                    With .Add("Size", SqlDbType.Date)
                        .Expression = "t.Size"
                    End With
                    With .Add("UPC_Code", SqlDbType.Date)
                        .Expression = "t.UPC_Code"
                    End With
                    With .Add("Inv_Balance_Qty", SqlDbType.Float)
                        .Expression = "SUM(t.balance)"
                    End With
                End With

                .SqlFrom = "FROM (SELECT DISTINCT " &
                            "dbo.MobizLibraryPIMProduct.ID, dbo.MobizLibraryPIMProduct.ProductNo AS UPC_Code, dbo.MobizLibraryPIMProduct.ProductName, " &
                            "dbo.MobizLibraryPIMProduct.ProductGroupNo AS Product_Code, dbo.MobizLibraryOrganizationFOWST.AccountID AS Account_ID,  " &
                            "dbo.MobizLibraryOrganizationFOWST.StoreID AS Store_ID, replace(convert(varchar, convert(date," & ReadyString(m_date) & "), 102),'.','') AS Inv_Balance_Date, dbo.MobizLibraryPIMProductVariableFeatures001.C0017 AS Size, " &
                            "bal1.Balance " &
                            "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " &
                            "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.LibraryOrganizationPIM.ProductID LEFT OUTER JOIN " &
                            "dbo.MobizLibraryOrganization INNER JOIN " &
                            "dbo.MobizLibraryOrganizationFOWST ON dbo.MobizLibraryOrganization.ID = dbo.MobizLibraryOrganizationFOWST.OrganizationID INNER JOIN " &
                            "dbo.TransInventoryLedger AS t0 INNER JOIN " &
                            "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProduct.ID = t0.ProductID ON dbo.MobizLibraryOrganizationFOWST.ID = t0.FowstID ON  " &
                            "dbo.LibraryOrganizationPIM.ProductID = t0.ProductID " &
                            "outer apply(  " &
                         "select top 1 Balance from TransInventoryLedger as t1 where t1.ProductID = t0.ProductID And t1.FowstID = t0.FowstID  " &
                         "And t1.BinID = t0.BinID order by LastUpdated desc )  bal1 	 " &
                         "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 " &
                         ") t   "
                .SqlGroup = "group by Account_ID, Store_ID, Inv_Balance_Date, UPC_Code, Size, Product_Code"

                pDT = mBusiness.ProData.Execute(.CreateSql)

                pDT.DefaultView.RowFilter = "Inv_Balance_Qty <> 0"
                'pDT = .Requery

                'Dim m_date As Date = CDate(WebRequest.Instance.Data("To"))

                TableToExcelFile(pDT.DefaultView.ToTable, "", "", Common.ServerPath & "temp\InvBal_Week_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

                'Return "InvBal_Week_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
                Return True
            End With
        End Function

        Function GetWeeklySalesReport(ByVal DateFrom As Date, ByVal DateTo As Date) As Boolean
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData

                With .Fields
                    With .Add("Account_ID", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID"
                    End With
                    With .Add("Store_ID", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID"
                    End With
                    With .Add("Transaction_ID", SqlDbType.VarChar)
                        .Expression = "tSource.TransName"
                    End With
                    With .Add("Transaction_Date", SqlDbType.Date)
                        .Expression = "replace(convert(varchar, tSource.TransactionDate, 102),'.','')"
                    End With
                    With .Add("Product_Code", SqlDbType.Float)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                    End With
                    With .Add("Size", SqlDbType.VarChar)
                        .Expression = "ISNULL(dbo.MobizLibraryPIMProductVariableFeatures001.C0017,'MISC')"
                    End With
                    With .Add("UPC_Code", SqlDbType.Int)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                    End With
                    With .Add("Sold_Qty", SqlDbType.Float)
                        .Expression = "dbo.TransInventoryLedger.Qty * -1"
                    End With
                    With .Add("Sold_Amt", SqlDbType.VarChar)
                        .Expression = "replace(CONVERT(varchar(12), tSource.VatExcluded, 1),',','')"
                    End With
                    With .Add("Sold_Amt_Discount", SqlDbType.VarChar)
                        .Expression = "replace(CONVERT(varchar(12), tSource.Discount, 1),',','')"
                    End With
                    With .Add("Transaction_Type", SqlDbType.VarChar)
                        .Expression = "dbo.TransInventoryLedger.TransactionType"
                    End With
                End With

                .SqlFrom = "FROM dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                            "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                            "dbo.TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = dbo.TransInventoryLedger.FowstID INNER JOIN " & vbCrLf &
                            "dbo.LibraryOrganizationPIM ON dbo.TransInventoryLedger.ProductID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                            "dbo.MobizLibraryPIMProduct LEFT JOIN " & vbCrLf &
                            "dbo.MobizLibraryPIMProductVariableFeatures001 ON dbo.MobizLibraryPIMProduct.ID = dbo.MobizLibraryPIMProductVariableFeatures001.ProductID ON  " & vbCrLf &
                            "dbo.LibraryOrganizationPIM.ProductID = dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                            "OUTER APPLY ( " & vbCrLf &
                            "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoiceItems.TaxSales as VatExcluded, TransInvoice.InvoiceDate as TransactionDate, TransInvoiceItems.DiscountSales as Discount  " & vbCrLf &
                            "FROM TransInvoice INNER JOIN TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                            "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                            "AND TransInventoryLedger.ProductID = TransInvoiceItems.ProductID " & vbCrLf &
                            "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                            "UNION " & vbCrLf &
                            "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, (dbo.TransMemoSalesReturn.TotalAmount - TransMemoSalesReturn.TaxAmount ) * -1 as VatExcluded, TransMemo.MemoDate as TransactionDate, '' as Discount  " & vbCrLf &
                            "FROM TransMemo " & vbCrLf &
                            "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                            "AND TransInventoryLedger.ProductID = TransMemoSalesReturn.ProductID " & vbCrLf &
                            "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                            "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                            ") tSource"

                .SqlWhere = "WHERE (TransactionType = 'Sales Returns' OR TransactionType = 'Invoice') AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

                .SqlOrder = "ORDER BY tSource.TransName"

                'pDT = .Requery
                pDT = mBusiness.ProData.Execute(.CreateSql)

                Dim m_date As Date = DateTo
                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\POS_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

                'Return "POS_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
                Return True
            End With

        End Function

        Function GetMonthInventoryReport(ByVal DateFrom As Date, ByVal DateTo As Date) As Boolean
            Dim pFinder As Finder
            Dim pDT As DataTable
            Dim SqlWhere As String

            pFinder = New Finder

            With pFinder
                .ProData = Common.WebRequest.ProData

                .Distinct = True
                With .Fields
                    With .Add("Account_ID", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.AccountID"
                    End With
                    With .Add("Store_ID", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryOrganizationFOWST.StoreID"
                    End With
                    With .Add("Transaction_ID", SqlDbType.VarChar)
                        .Expression = "ISNULL(tSource.TransName, TransInventoryLedger.TransactionType)"
                    End With
                    With .Add("Inv_Mvmt_Date", SqlDbType.Date)
                        .Expression = "replace(convert(date, tSource.TransactionDate),'-','')"
                    End With
                    With .Add("Product_Code", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductGroupNo"
                    End With
                    With .Add("Size", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryPIMProductVariableFeatures001.C0017"
                    End With
                    With .Add("UPC_Code", SqlDbType.VarChar)
                        .Expression = "dbo.MobizLibraryPIMProduct.ProductNo"
                    End With
                    With .Add("Inv_Mvmt_Qty", SqlDbType.Float)
                        .Expression = "TransInventoryLedger.Qty"
                    End With
                    With .Add("TransactionType", SqlDbType.Int)
                        .Expression = "TransInventoryLedger.TransactionType"
                    End With
                End With

                .SqlFrom = "FROM dbo.MobizLibraryPIMProductVariableFeatures001 INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryPIMProduct ON dbo.MobizLibraryPIMProductVariableFeatures001.ProductID = dbo.MobizLibraryPIMProduct.ID INNER JOIN " & vbCrLf &
                          "dbo.LibraryOrganizationPIM ON dbo.MobizLibraryPIMProduct.ID = dbo.LibraryOrganizationPIM.ProductID INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryOrganizationFOWST INNER JOIN " & vbCrLf &
                          "dbo.MobizLibraryOrganization ON dbo.MobizLibraryOrganizationFOWST.OrganizationID = dbo.MobizLibraryOrganization.ID INNER JOIN " & vbCrLf &
                          "dbo.TransInventoryLedger AS TransInventoryLedger ON dbo.MobizLibraryOrganizationFOWST.ID = TransInventoryLedger.FowstID ON  " & vbCrLf &
                          "dbo.LibraryOrganizationPIM.ProductID = TransInventoryLedger.ProductID " & vbCrLf &
                            "OUTER APPLY ( " & vbCrLf &
                          "SELECT CONVERT(varchar, TransInvoice.InvoiceNumber) AS TransName, TransInvoice.InvoiceDate As TransactionDate " & vbCrLf &
                             "FROM TransInvoice " & vbCrLf &
                             "WHERE TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                             "AND TransInventoryLedger.TransactionType = 'Invoice' " & vbCrLf &
                             "UNION " & vbCrLf &
                             "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                             "FROM TransReceiving " & vbCrLf &
                             "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                             "AND TransInventoryLedger.TransactionType = 'Receiving' " & vbCrLf &
                            "UNION " & vbCrLf &
                             "SELECT CONVERT(varchar, TransReceiving.ReceivingNo) AS TransName, TransReceiving.DateReceived As TransactionDate " & vbCrLf &
                             "FROM TransReceiving " & vbCrLf &
                             "WHERE TransReceiving.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                             "AND TransInventoryLedger.TransactionType = 'TransferOut' " & vbCrLf &
                            "UNION " & vbCrLf &
                             "SELECT CONVERT(varchar, TransMemo.MemoNumber) AS TransName, TransMemo.MemoDate As TransactionDate " & vbCrLf &
                             "FROM TransMemo " & vbCrLf &
                             "JOIN TransMemoSalesReturn ON TransMemoSalesReturn.MemoID = TransMemo.ID " & vbCrLf &
                             "WHERE TransMemo.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                             "AND TransInventoryLedger.TransactionType = 'Sales Returns' " & vbCrLf &
                            "UNION " & vbCrLf &
                             "SELECT replace(CONVERT(varchar, CONVERT(date,TransInventoryProductCount.InventoryCountDate)),'-','') AS TransName,  TransInventoryProductCount.InventoryCountDate As TransactionDate" & vbCrLf &
                             "FROM TransInventoryProductCount " & vbCrLf &
                             "WHERE TransInventoryProductCount.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                             "AND TransInventoryLedger.TransactionType = 'InventoryCount' " & vbCrLf &
                            ") tSource"

                .SqlWhere = "WHERE dbo.LibraryOrganizationPIM.IsDoNotShareTransDataToBrandOwner = 0 AND dbo.MobizLibraryOrganizationFOWST.IsIncludeSpecialReport = 1 AND CONVERT(date, tSource.TransactionDate) BETWEEN " & ReadyString(DateFrom) & " AND " & ReadyString(DateTo)

                .SqlOrder = "ORDER BY Inv_Mvmt_Date"

                pDT = mBusiness.ProData.Execute(.CreateSql)

                Dim m_date As Date = CDate(DateTo)

                TableToExcelFile(pDT, "", "", Common.ServerPath & "temp\InvMvmt_Month_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv")

                'Return "InvMvmt_Month_" & m_date.Year & Format(m_date.Month, "00") & Format(m_date.Day, "00") & ".csv"
                Return True
            End With

        End Function

        Function GetAllSubscriptionDatabase() As DataTable
            Dim pSql As String
            pSql = "select * from Librarysubscription"

            Return Common.WebRequest.ProData.Execute(pSql)

        End Function
    End Class
End Namespace
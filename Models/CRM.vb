Imports System.Web.Script.Serialization

Public Class CRM

    Public Property ID As String

    Function GetTeamInfo() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("TeamName", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM LibraryCRMTeams "
            .SqlWhere = " WHERE ID = " & ReadyString(ID)

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetTeamMembers() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryPeople.ID"
                End With
                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.firstname"
                End With
            End With

            .SqlFrom = "FROM LibraryCRMTeamMembers inner join MobizLibraryPeople on MobizLibraryPeople.ID = LibraryCRMTeamMembers.PeopleID"
            .SqlWhere = " WHERE LibraryCRMTeamMembers.TeamID = " & ReadyString(ID)

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetTeamCustomers() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "MobizLibraryOrganization.ID"
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryOrganization.CompanyName"
                End With
            End With

            .SqlFrom = "FROM LibraryCRMCustomers inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = LibraryCRMCustomers.CustomerID"
            .SqlWhere = " WHERE LibraryCRMCustomers.TeamID = " & ReadyString(ID)

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetTeamQuota() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("TeamID", SqlDbType.VarChar)
                .Add("MonthYear", SqlDbType.VarChar)
                .Add("Tier1Quota", SqlDbType.VarChar)
                .Add("Tier1Incentive", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM LibraryCRMTeamQuota "
            .SqlWhere = " WHERE LibraryCRMTeamQuota.TeamID = " & ReadyString(ID)

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function GetAllTeams() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        Dim SearchKeyword As String = Common.WebRequest.Data("SearchKeyword")

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("TeamName", SqlDbType.VarChar)
                .Add("Description", SqlDbType.VarChar)
                .Add("LastUpdated", SqlDbType.VarChar)
            End With

            .SqlFrom = "FROM LibraryCRMTeams "
            .SqlWhere = "WHERE ISNULL(Type, 'team') = " & ReadyString(Common.WebRequest.Data("Type"))
            If CStr2(SearchKeyword) <> "" Then
                .SqlWhere += " AND TeamName LIKE '%" & SearchKeyword & "%'"
            End If

            pDT = .Requery
        End With

        Return pDT
    End Function

    Function SaveTeam() As String
        Dim p As New InsertUpdate("LibraryCRMTeams")
        Dim pSql As String
        Dim strIDReturn As String

        p("TeamName") = ReadyString(IIf(Common.WebRequest.Data("TeamName") = vbNullString, "", Common.WebRequest.Data("TeamName")))
        p("Description") = ReadyString(IIf(Common.WebRequest.Data("Description") = vbNullString, "", Common.WebRequest.Data("Description")))

        If Not Common.WebRequest.Data("FOWST") = vbNullString Then
            p("TeamFOWST") = ReadyString(Common.WebRequest.Data("FOWST"))
        End If

        p("LastUpdated") = "GetDate()"
        p("Type") = ReadyString(Common.WebRequest.Data("Type"))

        Try
            Common.WebRequest.ProData.BeginTransaction()

            If ID.Trim().Length <= 0 Then

                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Delete from LibraryCRMTeamMembers where TeamID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Delete from LibraryCRMCustomers where TeamID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            pSql = "Delete from LibraryCRMTeamQuota where TeamID = " & ReadyString(strIDReturn)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            For Each pObj As Object In Common.WebRequest.AllData("Employees")
                p = New InsertUpdate("LibraryCRMTeamMembers")

                p("TeamID") = ReadyString(strIDReturn)
                p("PeopleID") = ReadyString(pObj("Employee"))
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            For Each pObj As Object In Common.WebRequest.AllData("Customers")
                p = New InsertUpdate("LibraryCRMCustomers")

                p("TeamID") = ReadyString(strIDReturn)
                p("CustomerID") = ReadyString(pObj("Customer"))
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            Next

            If IsNumeric(Common.WebRequest.Data("JanQuota")) And IsNumeric(Common.WebRequest.Data("JanIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")

                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("1/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("JanQuota")), CDbl(Common.WebRequest.Data("JanQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("JanIncentive")), CInt(Common.WebRequest.Data("JanIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("FebQuota")) And IsNumeric(Common.WebRequest.Data("FebIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("2/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("FebQuota")), CDbl(Common.WebRequest.Data("FebQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("FebIncentive")), CInt(Common.WebRequest.Data("FebIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("MarQuota")) And IsNumeric(Common.WebRequest.Data("MarIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("3/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("MarQuota")), CDbl(Common.WebRequest.Data("MarQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("MarIncentive")), CInt(Common.WebRequest.Data("MarIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("AprQuota")) And IsNumeric(Common.WebRequest.Data("AprIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("4/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("AprQuota")), CDbl(Common.WebRequest.Data("AprQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("AprIncentive")), CInt(Common.WebRequest.Data("AprIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("MayQuota")) And IsNumeric(Common.WebRequest.Data("MayIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("5/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("MayQuota")), CDbl(Common.WebRequest.Data("MayQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("MayIncentive")), CInt(Common.WebRequest.Data("MayIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("JunQuota")) And IsNumeric(Common.WebRequest.Data("JunIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("6/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("JunQuota")), CDbl(Common.WebRequest.Data("JunQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("JunIncentive")), CInt(Common.WebRequest.Data("JunIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("JulQuota")) And IsNumeric(Common.WebRequest.Data("JulIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("7/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("JulQuota")), CDbl(Common.WebRequest.Data("JulQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("JulIncentive")), CInt(Common.WebRequest.Data("JulIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("AugQuota")) And IsNumeric(Common.WebRequest.Data("AugIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("8/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("AugQuota")), CDbl(Common.WebRequest.Data("AugQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("AugIncentive")), CInt(Common.WebRequest.Data("AugIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("SepQuota")) And IsNumeric(Common.WebRequest.Data("SepIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("9/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("SepQuota")), CDbl(Common.WebRequest.Data("SepQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("SepIncentive")), CInt(Common.WebRequest.Data("SepIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("OctQuota")) And IsNumeric(Common.WebRequest.Data("OctIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("10/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("OctQuota")), CDbl(Common.WebRequest.Data("OctQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("OctIncentive")), CInt(Common.WebRequest.Data("OctIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("NovQuota")) And IsNumeric(Common.WebRequest.Data("NovIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("11/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("NovQuota")), CDbl(Common.WebRequest.Data("NovQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("NovIncentive")), CInt(Common.WebRequest.Data("NovIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            If IsNumeric(Common.WebRequest.Data("DecQuota")) And IsNumeric(Common.WebRequest.Data("DecIncentive")) Then
                p = New InsertUpdate("LibraryCRMTeamQuota")
                p("TeamID") = ReadyString(strIDReturn)
                p("MonthYear") = ReadyString("12/1/" & Year(Now))
                p("Tier1Quota") = IIf(IsNumeric(Common.WebRequest.Data("DecQuota")), CDbl(Common.WebRequest.Data("DecQuota")), 0)
                p("Tier1Incentive") = IIf(IsNumeric(Common.WebRequest.Data("DecIncentive")), CInt(Common.WebRequest.Data("DecIncentive")), 0)
                p("LastUpdated") = "getdate()"
                pSql = p.GetInsert()
                Common.WebRequest.ProData.ExecuteSQL(pSql)
            End If

            'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryDevice'"
            'Common.WebRequest.ProData.ExecuteSQL(pSql)

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function

    Function GetEmployeeSalesData() As DataTable
        Dim pFinder As Finder

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData
                
            With .Fields
                With .Add("Total", SqlDbType.Float)
                    .Expression = "SUM(Subtotal)"
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPeople.SurName + ', ' + MobizLibraryPeople.FirstName"
                End With

            End With

            .SqlFrom = "FROM TransInvoiceItems " & vbCrLf &
                    "inner join TransInvoice on TransInvoiceItems.InvoiceID = TransInvoice.ID " & vbCrLf &
                    "inner join LibraryCRMCustomers on LibraryCRMCustomers.CustomerID = TransInvoice.CustomerID  " & vbCrLf &
                    "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = transinvoice.CustomerID   " & vbCrLf &
                    "inner join MobizLibraryPeople on MobizLibraryPeople.ID = Transinvoice.UserID " & vbCrLf &
                    "inner join LibraryCRMTeamMembers on LibraryCRMTeamMembers.PeopleID = TransInvoice.UserID"

            .SqlWhere = " WHERE salesorderno IS NOT NULL AND LibraryCRMCustomers.TeamID = " & ReadyString(Common.WebRequest.Data("Fowst")) & vbCrLf &
                                " AND Month(Transinvoice.InvoiceDate) = " & CDate(Common.WebRequest.Data("To")).Month & vbCrLf &
                                " AND YEAR(Transinvoice.InvoiceDate) = " & Today.Year

            .SqlGroup = "GROUP BY MobizLibraryPeople.SurName, MobizLibraryPeople.FirstName "
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function GetSalesOrderData() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable
        Dim pDtFinal As DataTable

        If Common.WebRequest.Data("GroupBy") = "Month" Then
            Dim mWeeks(7) As Date
            Dim mDate As Date
            Dim IsLastWeek As Boolean = False
            Dim TeamID As String
            Dim mSalesQuota As Double
            Dim mSalesQuotaPerDay As Double
            Dim mDateDifference As Integer
            Dim mNoOfDays As Integer

            TeamID = Common.WebRequest.Data("Fowst")
            mDate = CDate(Common.WebRequest.Data("To"))
            mWeeks(0) = CDate(Month(mDate) & "/1/" & Year(mDate))

            mSalesQuota = GetThisMonthQuota(mDate, TeamID)
            mSalesQuotaPerDay = mSalesQuota / DateAdd(DateInterval.Day, -1, DateAdd(DateInterval.Month, 1, mWeeks(0))).Day

            For i As Integer = 0 To 6
                Select Case (mWeeks(i).DayOfWeek)
                    Case DayOfWeek.Sunday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 7, mWeeks(i))
                    Case DayOfWeek.Monday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 6, mWeeks(i))
                    Case DayOfWeek.Tuesday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 5, mWeeks(i))
                    Case DayOfWeek.Wednesday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 4, mWeeks(i))
                    Case DayOfWeek.Thursday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 3, mWeeks(i))
                    Case DayOfWeek.Friday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 2, mWeeks(i))
                    Case DayOfWeek.Saturday
                        mWeeks(i + 1) = DateAdd(DateInterval.Day, 1, mWeeks(i))
                End Select

                If DateDiff(DateInterval.Month, mWeeks(i), mWeeks(i + 1)) > 0 Then
                    If Month(mDate) = 12 Then
                        mWeeks(i + 1) = "1/1/" & (Year(mDate) + 1)
                    Else
                        mWeeks(i + 1) = CDate((Month(mDate) + 1) & "/1/" & Year(mDate))
                    End If

                    IsLastWeek = True
                End If

                mDateDifference = DateDiff(DateInterval.Day, mWeeks(i), mWeeks(i + 1))

                'get data per week/month
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        .Add("SalesOrderNo", SqlDbType.Int)
                        .Add("Invoicedate", SqlDbType.DateTime2)
                        .Add("CompanyName", SqlDbType.VarChar)
                        With .Add("Total", SqlDbType.UniqueIdentifier)
                            .Expression = "SUM(Subtotal)"
                        End With
                        With .Add("Week", SqlDbType.UniqueIdentifier)
                            .Expression = i + 1
                        End With

                        .Add("Tier1Quota", SqlDbType.Float)

                        With .Add("QuotaPerWeek", SqlDbType.Float)
                            .Expression = mSalesQuotaPerDay * mDateDifference
                        End With
                    End With

                    .SqlFrom = "FROM Transinvoice " & vbCrLf &
                                "inner join Transinvoiceitems  " & vbCrLf &
                                "on Transinvoiceitems.invoiceid = Transinvoice.id " & vbCrLf &
                                "inner join LibraryCRMCustomers on LibraryCRMCustomers.CustomerID = TransInvoice.CustomerID " & vbCrLf &
                                "inner join LibraryCRMTeamQuota on LibraryCRMCustomers.TeamID = LibraryCRMTeamQuota.TeamID " & vbCrLf &
                                "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = transinvoice.CustomerID"

                    .SqlWhere = " WHERE salesorderno IS NOT NULL " & vbCrLf &
                                " AND LibraryCRMCustomers.TeamID = " & ReadyString(TeamID) & vbCrLf &
                                " AND convert(date, Transinvoice.InvoiceDate) between " & ReadyString(mWeeks(i)) & vbCrLf &
                                " AND " & ReadyString(DateAdd(DateInterval.Day, -1, mWeeks(i + 1))) & vbCrLf &
                                " AND MONTH(LibraryCRMTeamQuota.MonthYear) = " & Month(mWeeks(i)) & vbCrLf &
                                " AND YEAR(LibraryCRMTeamQuota.MonthYear) = " & Year(mWeeks(i))
                    .SqlGroup = " GROUP BY Tier1Quota, SalesOrderNo, Invoicedate, CompanyName"
                    pDT = .Requery

                    If i = 0 Then
                        'to copy rows and column name
                        pDtFinal = pDT

                        If pDT.Rows.Count = 0 Then
                            Dim dr As DataRow
                            dr = pDtFinal.NewRow

                            dr(0) = "0"
                            dr(1) = Today
                            dr(2) = "Nothing"
                            dr(3) = 0
                            dr(4) = i + 1
                            dr(5) = GetThisMonthQuota(mDate, TeamID)
                            dr(6) = mSalesQuotaPerDay * mDateDifference
                            pDtFinal.Rows.Add(dr)
                        End If

                    Else
                        If pDT.Rows.Count > 0 Then
                            For Each dr As DataRow In pDT.Rows
                                pDtFinal.ImportRow(dr)
                            Next
                        Else
                            Dim dr As DataRow
                            dr = pDtFinal.NewRow

                            dr(0) = "0"
                            dr(1) = Today
                            dr(2) = "Nothing"
                            dr(3) = 0
                            dr(4) = i + 1
                            dr(5) = GetThisMonthQuota(mDate, TeamID)
                            dr(6) = mSalesQuotaPerDay * mDateDifference
                            pDtFinal.Rows.Add(dr)
                        End If
                    End If

                    If IsLastWeek Then
                        Exit For
                    End If
                End With
            Next
        Else
            Dim MonthTo As Integer
            Dim StartingMonth As Integer
            Dim TeamID As String

            TeamID = Common.WebRequest.Data("Fowst")

            If Common.WebRequest.Data("GroupBy") = "Quarter" Then
                MonthTo = 2
                Select Case Common.WebRequest.Data("To")
                    Case "Q1"
                        StartingMonth = 1
                    Case "Q2"
                        StartingMonth = 4
                    Case "Q3"
                        StartingMonth = 7
                    Case "Q4"
                        StartingMonth = 10
                End Select
            Else
                StartingMonth = 1
                MonthTo = 11
            End If

            For i As Integer = 0 To MonthTo

                'get data per month
                pFinder = New Finder
                With pFinder
                    .ProData = Common.WebRequest.ProData

                    With .Fields
                        .Add("SalesOrderNo", SqlDbType.Int)
                        .Add("Invoicedate", SqlDbType.DateTime2)
                        .Add("CompanyName", SqlDbType.VarChar)
                        With .Add("Total", SqlDbType.UniqueIdentifier)
                            .Expression = "SUM(Subtotal)"
                        End With
                        With .Add("Month", SqlDbType.UniqueIdentifier)
                            .Expression = StartingMonth
                        End With
                        .Add("Tier1Quota", SqlDbType.Float)
                    End With

                    .SqlFrom = "FROM Transinvoice " & vbCrLf &
                                "inner join Transinvoiceitems  " & vbCrLf &
                                "on Transinvoiceitems.invoiceid = Transinvoice.id " & vbCrLf &
                                "inner join LibraryCRMCustomers on LibraryCRMCustomers.CustomerID = TransInvoice.CustomerID " & vbCrLf &
                                "inner join LibraryCRMTeamQuota on LibraryCRMCustomers.TeamID = LibraryCRMTeamQuota.TeamID " & vbCrLf &
                                "inner join MobizLibraryOrganization on MobizLibraryOrganization.ID = transinvoice.CustomerID"

                    .SqlWhere = " WHERE salesorderno IS NOT NULL AND LibraryCRMCustomers.TeamID = " & ReadyString(TeamID) & vbCrLf &
                                " AND MONTH(Transinvoice.InvoiceDate) = " & StartingMonth &
                                " AND YEAR(Transinvoice.InvoiceDate) = " & Today.Year &
                                " AND MONTH(LibraryCRMTeamQuota.MonthYear) = " & StartingMonth & vbCrLf &
                                " AND YEAR(LibraryCRMTeamQuota.MonthYear) = " & Today.Year
                    .SqlGroup = " GROUP BY Tier1Quota, SalesOrderNo, Invoicedate, CompanyName"

                    pDT = .Requery

                    If i = 0 Then
                        'to copy rows and column name
                        pDtFinal = pDT

                        If pDT.Rows.Count = 0 Then
                            Dim dr As DataRow
                            dr = pDtFinal.NewRow

                            dr(0) = "0"
                            dr(1) = Today
                            dr(2) = "Nothing"
                            dr(3) = 0
                            dr(4) = StartingMonth
                            dr(5) = GetThisMonthQuota(CDate(StartingMonth & "/1/" & Today.Year), TeamID)
                            pDtFinal.Rows.Add(dr)
                        End If
                    Else
                        If pDT.Rows.Count > 0 Then
                            For Each dr As DataRow In pDT.Rows
                                pDtFinal.ImportRow(dr)
                            Next
                        Else
                            Dim dr As DataRow
                            dr = pDtFinal.NewRow

                            dr(0) = "0"
                            dr(1) = Today
                            dr(2) = "Nothing"
                            dr(3) = 0
                            dr(4) = StartingMonth
                            dr(5) = GetThisMonthQuota(CDate(StartingMonth & "/1/" & Today.Year), TeamID)
                            pDtFinal.Rows.Add(dr)
                        End If
                    End If

                    StartingMonth += 1
                End With
            Next
            End If

            Return pDtFinal
    End Function

    Function GetThisMonthQuota(ByVal mDate As Date, ByVal TeamID As String) As Double
        Dim pDt As New DataTable

        pSql = "Select Tier1Quota from LibraryCRMTeamQuota where " &
            " TeamID = " & ReadyString(TeamID) & vbCrLf &
            " AND MONTH(LibraryCRMTeamQuota.MonthYear) = " & mDate.Month & vbCrLf &
            " AND YEAR(LibraryCRMTeamQuota.MonthYear) = " & mDate.Year

        pDt = Common.WebRequest.ProData.Execute(pSql)

        If pDt.Rows.Count > 0 Then
            Return pDt.Rows(0).Item("Tier1Quota")
        Else
            Return 0
        End If
    End Function

#Region "OldCode"

    Dim pSql As String = vbNullString
    Dim pDT As DataTable

    Dim dtCloseDeal As New DataTable
    Dim dtToProspect As New DataTable
    Dim dtLeads As New DataTable

    Dim ToLeads As Integer
    Dim ToCloseDeal As Integer
    Dim ToProspect As Integer

    Sub GetAllCustomers()

        pSql = "SELECT TOP (100) PERCENT dbo.LibraryOrganization.ID, dbo.LibraryOrganization.CorporateName, dbo.LibraryOrganization.BusinessTradeName, " &
                "dbo.LibraryOrganization.TIN, " &
                "(SELECT     TOP (1) DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice " &
                "WHERE CustomerID = dbo.LibraryOrganization.ID " &
                "ORDER BY InvoiceDate DESC) AS DaysLastPurchase,  " &
                "dbo.LibraryOrganization.BusinessAddressStreet + ', ' + dbo.LibraryCities.Name + ' ' + dbo.LibraryCountries.Name AS Address,  " &
                "dbo.LibraryOrganization.OwnershipType " &
                "FROM dbo.LibraryOrganization INNER JOIN " &
                "dbo.LibraryCities ON dbo.LibraryOrganization.BusinessAddressCityID = dbo.LibraryCities.ID INNER JOIN " &
                "dbo.LibraryCountries ON dbo.LibraryOrganization.BusinessAddressCountryID = dbo.LibraryCountries.ID LEFT OUTER JOIN " &
                "dbo.TransInvoice AS TransInvoice_1 ON dbo.LibraryOrganization.ID = TransInvoice_1.CustomerID LEFT OUTER JOIN " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = TransInvoice_1.CustomerID " &
                "ORDER BY DaysLastPurchase DESC"
        '"where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & 5  WebSession.Instance.EmployeeID

        With Common.WebRequest
            WebSession.Instance.dtCustomer = .ProData.Execute(pSql)
        End With

    End Sub

    Function getTeamID() As Boolean
        Dim dt As New DataTable

        pSql = "SELECT DISTINCT " _
            & "dbo.MainSecurityAccess.EmployeeID AS ID, A.LastName + ', ' + A.FirstName + ' ' + A.MiddleName AS EmployeeName, " _
            & "dbo.MainSecurityAccess.Password, DavaoUnifiedPayroll.dbo.IncentiveTeams.TeamName, DavaoUnifiedPayroll.dbo.LibraryBranch.Name,  " _
            & "DavaoUnifiedPayroll.dbo.IncentiveTeams.ID AS TeamID, dbo.MainSecurityLevel.LevelType, dbo.MainSecurityAccess.IsCancelled, " _
            & "DavaoUnifiedPayroll.dbo.IncentiveTeams.isGetSalesFromPOS, DavaoUnifiedPayroll.dbo.LibraryBranch.ID AS BranchID " _
            & "FROM DavaoUnifiedPayroll.dbo.IncentiveTeamMembers INNER JOIN DavaoUnifiedPayroll.dbo.IncentiveTeams ON  " _
            & "DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = DavaoUnifiedPayroll.dbo.IncentiveTeams.ID INNER JOIN " _
            & "dbo.MainSecurityAccess INNER JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles A ON dbo.MainSecurityAccess.EmployeeID = A.ID INNER JOIN " _
            & "dbo.MainSecurityLevel ON dbo.MainSecurityLevel.ID = dbo.MainSecurityAccess.LevelID ON  " _
            & "DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = A.ID INNER JOIN DavaoUnifiedPayroll.dbo.LibraryBranch ON  " _
            & "DavaoUnifiedPayroll.dbo.IncentiveTeams.OfficeLocation = DavaoUnifiedPayroll.dbo.LibraryBranch.ID " _
            & "WHERE(dbo.MainSecurityAccess.EmployeeID = " & WebSession.Instance.EmployeeID & ")"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        If Not pDT Is Nothing Then
            If pDT.Rows.Count > 0 Then
                WebSession.Instance.TeamID = CInt(pDT.Rows(0).Item("TeamID"))
                Return True
                Exit Function
            End If
        End If
        Return False
    End Function

    Function isAdmin() As String
        Dim dt As New DataTable

        pSql = "Select DISTINCT " _
                & "dbo.MainSecurityAccess.EmployeeID AS ID, A.LastName + ', ' + A.FirstName + ' ' + A.MiddleName AS EmployeeName, " _
                & "dbo.MainSecurityAccess.Password, dbo.MainSecurityLevel.LevelType " _
                & "FROM dbo.MainSecurityAccess INNER JOIN " _
                & "DavaoUnifiedPayroll.dbo.EmployeeProfiles A ON dbo.MainSecurityAccess.EmployeeID = A.ID INNER JOIN " _
                & "dbo.MainSecurityLevel ON dbo.MainSecurityLevel.ID = dbo.MainSecurityAccess.LevelID " _
                & "WHERE(dbo.MainSecurityAccess.EmployeeID = " & WebSession.Instance.EmployeeID & ")" & " AND (dbo.MainSecurityAccess.IsCancelled = 0) AND " _
                & "LevelType = 'ADMINISTRATOR'"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        If pDT.Rows.Count > 0 Then
            Return "True"
        Else
            Return "False"
        End If
    End Function

    Function GetCustomersContactDetails() As DataTable
        pSql = "SELECT DISTINCT ID, FirstName + ' ' + LastName AS Name " &
                "FROM dbo.LibraryCustomersRepresentatives WHERE CustomerID = " & ReadyString(Common.WebRequest.Data("CustomerID"))

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT
    End Function

    Function GetCustomerCount() As DataTable
        Dim dt As New DataTable
        Dim dr As DataRow
        Dim ctr As Integer
        Dim dtDaysInterval As New DataTable

        dt.Columns.Add("Days")
        dt.Columns.Add("Value")

        dtDaysInterval = getDaysInterval()

        ToLeads = dtDaysInterval.Rows(1).Item(2)
        ToProspect = dtDaysInterval.Rows(2).Item(2)
        ToCloseDeal = dtDaysInterval.Rows(0).Item(2)

        While ctr < 3
            pSql = "SELECT COUNT(*) AS Count " &
                "FROM DavaoUnifiedPayroll.dbo.IncentiveScopeDetails INNER JOIN " &
                "dbo.LibraryCustomers ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN " &
                "dbo.TransInvoice AS TransInvoice_1 ON dbo.LibraryCustomers.ID = TransInvoice_1.CustomerID "

            dr = dt.NewRow

            Select Case ctr

                Case 0
                    pSql += "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                    " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                    "FROM dbo.TransInvoice AS TransInvoice_2 " &
                    "WHERE (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 0) " &
                    "and CustomerID = dbo.LibraryCustomers.ID " &
                    "ORDER BY InvoiceDate DESC) >= " & ToProspect & ") OR " &
                    "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                    "FROM dbo.TransInvoice AS TransInvoice_2 " &
                    "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                    "ORDER BY InvoiceDate DESC) IS NULL "
                    dr(0) = ToLeads
                Case 1
                    pSql += "where ((DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 1) or (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 2)) or  (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                    " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                    "FROM dbo.TransInvoice AS TransInvoice_2 " &
                    "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                    "ORDER BY InvoiceDate DESC) >= " & ToCloseDeal & ") " &
                    " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                    "FROM dbo.TransInvoice AS TransInvoice_2 " &
                    "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                    "ORDER BY InvoiceDate DESC) <= " & ToProspect & ")) "
                    dr(0) = ToProspect
                Case 2
                    pSql += "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                    " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                    "FROM dbo.TransInvoice AS TransInvoice_2 " &
                    "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                    "ORDER BY InvoiceDate DESC) >= 0) AND " &
                    "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                    "FROM dbo.TransInvoice AS TransInvoice_2 " &
                    "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                    "ORDER BY InvoiceDate DESC) < " & ToCloseDeal
                    dr(0) = ToCloseDeal
            End Select

            With Common.WebRequest
                pDT = .ProData.Execute(pSql)
            End With

            dr(1) = pDT.Rows(0).Item("Count")

            dt.Rows.Add(dr)
            ctr += 1
        End While

        Return dt
    End Function

    Function GetCustomers(ByVal ToWhere As String) As DataTable
        Dim dtDaysInterval As New DataTable

        pSql = "SELECT DISTINCT TOP 100 dbo.LibraryCustomers.ID, " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) AS [DaysSinceLastPurchase], dbo.LibraryCustomers.CompanyName AS [CompanyName], dbo.LibraryCustomers.ContactNumber,  " &
                "dbo.LibraryBusinessTypes.BusinessType, dbo.LibraryDCT.Text AS DistributionChannel, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedValue, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedClosedDate, " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating " &
                "FROM DavaoUnifiedPayroll.dbo.IncentiveScopeDetails INNER JOIN " &
                "dbo.LibraryCustomers ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN " &
                "dbo.TransInvoice AS TransInvoice_1 ON dbo.LibraryCustomers.ID = TransInvoice_1.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryBusinessTypes ON dbo.LibraryCustomers.BusinessTypeID = dbo.LibraryBusinessTypes.ID LEFT OUTER JOIN " &
                "dbo.LibraryDCT ON dbo.LibraryBusinessTypes.DCTID = dbo.LibraryDCT.ID "


        dtDaysInterval = getDaysInterval()

        ToLeads = dtDaysInterval.Rows(1).Item(2)
        ToProspect = dtDaysInterval.Rows(2).Item(2)
        ToCloseDeal = dtDaysInterval.Rows(0).Item(2)


        Select Case ToWhere
            Case "Leads"
                pSql += "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 0) " &
                "and CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) >= " & ToProspect & ") OR " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) IS NULL " &
                " ORDER BY [DaysSinceLastPurchase] DESC"
            Case "Prospects"
                pSql += "where ((DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 1) or (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 2)) or  (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) >= " & ToCloseDeal & ") " &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) <= " & ToProspect & ")) " &
                " ORDER BY [DaysSinceLastPurchase] DESC"
            Case "CloseDeal"
                pSql += "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) >= 0) AND " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) < " & ToCloseDeal &
                " ORDER BY [DaysSinceLastPurchase] DESC"
            Case "Search"
                Dim strSplit() As String
                Dim ctrSplit As Integer = 1
                strSplit = Split(Common.WebRequest.Data("SearchString"), " ")

                pSql += "where CompanyName like " & ReadyStringWild(strSplit(0)) 'DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID & "AND 

                While ctrSplit <= UBound(strSplit)
                    pSql += " AND CompanyName like " & ReadyStringWild(strSplit(ctrSplit))
                    ctrSplit += 1
                End While

            Case Else
                pSql += "where " &
                    " dbo.LibraryCustomers.ID = " & ReadyString(Common.WebRequest.Data("CustomerID"))
                'DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID & " AND"
        End Select

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT
    End Function

    Function getLeads() As DataTable
        Dim dtDaysInterval As New DataTable

        dtDaysInterval = getDaysInterval()

        ToLeads = dtDaysInterval.Rows(1).Item(2)
        ToProspect = dtDaysInterval.Rows(2).Item(2)
        ToCloseDeal = dtDaysInterval.Rows(0).Item(2)

        pSql = "SELECT DISTINCT TOP 100 dbo.LibraryCustomers.ID, " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) AS [Days Since Last Purchase], dbo.LibraryCustomers.CompanyName AS [Company Name],  " &
                "dbo.LibraryBusinessTypes.BusinessType, dbo.LibraryDCT.Text AS DistributionChannel, dbo.LibraryStreetNames.StreetName AS Street,  " &
                "dbo.LibraryDistricts.Name AS District, dbo.LibraryCities.Name AS City, dbo.LibraryProvinces.Name AS Province,  " &
                "dbo.LibraryCountries.Name AS Country, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedValue, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedClosedDate, " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating " &
                "FROM DavaoUnifiedPayroll.dbo.IncentiveScopeDetails INNER JOIN " &
                "dbo.LibraryCustomers ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN " &
                "dbo.TransInvoice AS TransInvoice_1 ON dbo.LibraryCustomers.ID = TransInvoice_1.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryStreetNames INNER JOIN " &
                "dbo.LibraryDistricts ON dbo.LibraryStreetNames.DistrictID = dbo.LibraryDistricts.ID INNER JOIN " &
                "dbo.LibraryCities ON dbo.LibraryDistricts.CityID = dbo.LibraryCities.ID INNER JOIN " &
                "dbo.LibraryProvinces ON dbo.LibraryCities.ProvinceID = dbo.LibraryProvinces.ID INNER JOIN " &
                "dbo.LibraryCustomersAddresses ON dbo.LibraryStreetNames.ID = dbo.LibraryCustomersAddresses.StreetID LEFT OUTER JOIN " &
                "dbo.LibraryCountries ON dbo.LibraryProvinces.CountryID = dbo.LibraryCountries.ID ON  " &
                "dbo.LibraryCustomers.ID = dbo.LibraryCustomersAddresses.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryBusinessTypes ON dbo.LibraryCustomers.BusinessTypeID = dbo.LibraryBusinessTypes.ID FULL OUTER JOIN " &
                "dbo.LibraryDCT ON dbo.LibraryBusinessTypes.DCTID = dbo.LibraryDCT.ID " &
                "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 0) " &
                "and CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) >= " & ToProspect & ") OR " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) IS NULL " &
                " ORDER BY [Days Since Last Purchase] DESC"


        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT
    End Function

    Function getProspects() As DataTable
        Dim dtDaysInterval As New DataTable

        dtDaysInterval = getDaysInterval()

        ToLeads = dtDaysInterval.Rows(1).Item(2)
        ToProspect = dtDaysInterval.Rows(2).Item(2)
        ToCloseDeal = dtDaysInterval.Rows(0).Item(2)

        pSql = "SELECT DISTINCT TOP 100 dbo.LibraryCustomers.ID, " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) AS [Days Since Last Purchase], dbo.LibraryCustomers.CompanyName AS [Company Name],  " &
                "dbo.LibraryBusinessTypes.BusinessType, dbo.LibraryDCT.Text AS DistributionChannel, dbo.LibraryStreetNames.StreetName AS Street,  " &
                "dbo.LibraryDistricts.Name AS District, dbo.LibraryCities.Name AS City, dbo.LibraryProvinces.Name AS Province,  " &
                "dbo.LibraryCountries.Name AS Country, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedValue, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedClosedDate, " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating " &
                "FROM DavaoUnifiedPayroll.dbo.IncentiveScopeDetails INNER JOIN " &
                "dbo.LibraryCustomers ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN " &
                "dbo.TransInvoice AS TransInvoice_1 ON dbo.LibraryCustomers.ID = TransInvoice_1.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryStreetNames INNER JOIN " &
                "dbo.LibraryDistricts ON dbo.LibraryStreetNames.DistrictID = dbo.LibraryDistricts.ID INNER JOIN " &
                "dbo.LibraryCities ON dbo.LibraryDistricts.CityID = dbo.LibraryCities.ID INNER JOIN " &
                "dbo.LibraryProvinces ON dbo.LibraryCities.ProvinceID = dbo.LibraryProvinces.ID INNER JOIN " &
                "dbo.LibraryCustomersAddresses ON dbo.LibraryStreetNames.ID = dbo.LibraryCustomersAddresses.StreetID LEFT OUTER JOIN " &
                "dbo.LibraryCountries ON dbo.LibraryProvinces.CountryID = dbo.LibraryCountries.ID ON  " &
                "dbo.LibraryCustomers.ID = dbo.LibraryCustomersAddresses.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryBusinessTypes ON dbo.LibraryCustomers.BusinessTypeID = dbo.LibraryBusinessTypes.ID FULL OUTER JOIN " &
                "dbo.LibraryDCT ON dbo.LibraryBusinessTypes.DCTID = dbo.LibraryDCT.ID " &
                "where ((DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 1) or (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating = 2)) or  (DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) >= " & ToCloseDeal & ") " &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) <= " & ToProspect & ")) " &
                " ORDER BY [Days Since Last Purchase] DESC"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT

    End Function

    Function getClosedDeals() As DataTable
        Dim dtDaysInterval As New DataTable

        dtDaysInterval = getDaysInterval()

        ToLeads = dtDaysInterval.Rows(1).Item(2)
        ToProspect = dtDaysInterval.Rows(2).Item(2)
        ToCloseDeal = dtDaysInterval.Rows(0).Item(2)

        pSql = "SELECT DISTINCT TOP 100 dbo.LibraryCustomers.ID, " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) AS [Days Since Last Purchase], dbo.LibraryCustomers.CompanyName AS [Company Name],  " &
                "dbo.LibraryBusinessTypes.BusinessType, dbo.LibraryDCT.Text AS DistributionChannel, dbo.LibraryStreetNames.StreetName AS Street,  " &
                "dbo.LibraryDistricts.Name AS District, dbo.LibraryCities.Name AS City, dbo.LibraryProvinces.Name AS Province,  " &
                "dbo.LibraryCountries.Name AS Country, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedValue, DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedClosedDate, " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating " &
                "FROM DavaoUnifiedPayroll.dbo.IncentiveScopeDetails INNER JOIN " &
                "dbo.LibraryCustomers ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN " &
                "dbo.TransInvoice AS TransInvoice_1 ON dbo.LibraryCustomers.ID = TransInvoice_1.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryStreetNames INNER JOIN " &
                "dbo.LibraryDistricts ON dbo.LibraryStreetNames.DistrictID = dbo.LibraryDistricts.ID INNER JOIN " &
                "dbo.LibraryCities ON dbo.LibraryDistricts.CityID = dbo.LibraryCities.ID INNER JOIN " &
                "dbo.LibraryProvinces ON dbo.LibraryCities.ProvinceID = dbo.LibraryProvinces.ID INNER JOIN " &
                "dbo.LibraryCustomersAddresses ON dbo.LibraryStreetNames.ID = dbo.LibraryCustomersAddresses.StreetID LEFT OUTER JOIN " &
                "dbo.LibraryCountries ON dbo.LibraryProvinces.CountryID = dbo.LibraryCountries.ID ON  " &
                "dbo.LibraryCustomers.ID = dbo.LibraryCustomersAddresses.CustomerID LEFT OUTER JOIN " &
                "dbo.LibraryBusinessTypes ON dbo.LibraryCustomers.BusinessTypeID = dbo.LibraryBusinessTypes.ID FULL OUTER JOIN " &
                "dbo.LibraryDCT ON dbo.LibraryBusinessTypes.DCTID = dbo.LibraryDCT.ID " &
                "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID &
                " and ((SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) >= 0) AND " &
                "(SELECT TOP 1 DATEDIFF(d, InvoiceDate, GETDATE()) AS Expr1 " &
                "FROM dbo.TransInvoice AS TransInvoice_2 " &
                "WHERE CustomerID = dbo.LibraryCustomers.ID " &
                "ORDER BY InvoiceDate DESC) < " & ToCloseDeal &
                " ORDER BY [Days Since Last Purchase] DESC"

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT

    End Function

    Function getDaysInterval() As DataTable
        Dim sql As String

        sql = "Select * from SystemDefaults where Description = 'To Leads' or Description = 'To Close Deal' or Description = 'To Prospect'"

        With Common.WebRequest
            Return .ProData.Execute(sql)
        End With
    End Function

    Function convertToJSON(ByVal dt As DataTable, ByVal source As String) As String
        Dim ctr As Integer
        Dim str As String
        Dim rating As String
        Dim estClosedDate As String

        str = "{""aaData"": [ "

        While ctr < dt.Rows.Count

            Select Case dt.Rows(ctr).Item(12).ToString
                Case "1"
                    rating = "Warm"
                Case "2"
                    rating = "Hot"
                Case Else
                    Select Case source
                        Case "Prospects", "ClosedDeals"
                            rating = "Warm"
                        Case Else
                            rating = "Cold"
                    End Select

            End Select

            Select Case dt.Rows(ctr).Item(11).ToString
                Case "", "1/1/1900 12:00:00 AM"
                    estClosedDate = "N/A"
                Case Else
                    estClosedDate = FormatDateTime(CDate(dt.Rows(ctr).Item(11).ToString), DateFormat.ShortDate)
            End Select

            If ctr = 0 Then
                str = str & "[""" & dt.Rows(ctr).Item(1).ToString &
                     """,""<empInfo id='" & dt.Rows(ctr).Item(0).ToString & "'class='employeeInformation'>" & Replace(dt.Rows(ctr).Item(2).ToString, "\", "/") & "</empInfo>" &
                     """,""" & dt.Rows(ctr).Item(5).ToString + ", " + dt.Rows(ctr).Item(6).ToString + ", " + dt.Rows(ctr).Item(7).ToString &
                     """,""<mValue name=" & source & " id='" & dt.Rows(ctr).Item(0).ToString & "'class='myValue'> " & IIf(dt.Rows(ctr).Item(10).ToString = "0.00", "N/A", dt.Rows(ctr).Item(10).ToString) & " </mValue>" &
                     """,""<mDate name=" & source & " id='" & dt.Rows(ctr).Item(0).ToString & "'class='myDate'> " & estClosedDate & " </mDate>" &
                     """,""<mRating name=" & source & " id='" & dt.Rows(ctr).Item(0).ToString & "'class='myRating'> " & rating & " </mRating>""]"
            Else
                str = str & ",[""" & dt.Rows(ctr).Item(1).ToString &
                     """,""<empInfo id='" & dt.Rows(ctr).Item(0).ToString & "'class='employeeInformation'>" & Replace(dt.Rows(ctr).Item(2).ToString, "\", "/") & "</empInfo>" &
                     """,""" & dt.Rows(ctr).Item(5).ToString + ", " + dt.Rows(ctr).Item(6).ToString + ", " + dt.Rows(ctr).Item(7).ToString &
                     """,""<mValue name=" & source & " id='" & dt.Rows(ctr).Item(0).ToString & "'class='myValue'> " & IIf(dt.Rows(ctr).Item(10).ToString = "0.00", "N/A", dt.Rows(ctr).Item(10).ToString) & "</mValue>" &
                     """,""<mDate name=" & source & " id='" & dt.Rows(ctr).Item(0).ToString & "'class='myDate'> " & estClosedDate & " </mDate>" &
                     """,""<mRating name=" & source & " id='" & dt.Rows(ctr).Item(0).ToString & "'class='myRating'> " & rating & " </mRating>""]"
            End If
            ctr += 1
        End While

        str = str & "],"

        str = str &
         """aoColumns"": [" &
            "{ ""sTitle"": ""Days Since Last Purchase"", ""sClass"": ""center""}," &
            "{ ""sTitle"": ""Company Name""}," &
            "{ ""sTitle"": ""Address""}," &
            "{ ""sTitle"": ""Est. Value/Month"", ""sClass"": ""center""}," &
            "{ ""sTitle"": ""Est. Close Date"", ""sClass"": ""center""}," &
            "{ ""sTitle"": ""Rating"", ""sClass"": ""center""}"
        str = str & "]}"

        Return str
    End Function

    Function convertTodoTableToJSON(ByVal dt As DataTable) As String
        Dim ctr As Integer
        Dim str As String

        str = "{""aaData"": [ "

        str = str & "[""" & "1" &
             """,""<empInfo id='1' class='employeeInformation'>11</empInfo>" &
             """,""" & "1"

        While ctr < dt.Rows.Count
            'If ctr = 0 Then
            '    str = str & "[""" & dt.Rows(ctr).Item(1).ToString &
            '         """,""<empInfo id='" & dt.Rows(ctr).Item(0).ToString & "'class='employeeInformation'>" & Replace(dt.Rows(ctr).Item(2).ToString, "\", "/") & "</empInfo>" &
            '         """,""" & dt.Rows(ctr).Item(3).ToString
            'Else
            str = str & ",[""" & dt.Rows(ctr).Item(1).ToString &
                 """,""<empInfo id='" & dt.Rows(ctr).Item(0).ToString & "'class='employeeInformation'>" & Replace(dt.Rows(ctr).Item(2).ToString, "\", "/") & "</empInfo>" &
                 """,""" & dt.Rows(ctr).Item(3).ToString
            'End If
            ctr += 1
        End While

        str = str & "],"

        str = str &
         """aoColumns"": [" &
            "{ ""sTitle"": ""Company Name"", ""sClass"": ""center""}," &
            "{ ""sTitle"": ""Order No"", ""sClass"": ""center""}," &
            "{ ""sTitle"": ""Contact Number"", ""sClass"": ""center""}"
        str = str & "]}"

        Return str
    End Function

    Function saveRating(ByVal sdt As Dictionary(Of String, Object)) As Boolean
        Dim sql As String
        Dim myRating As Integer

        Select Case sdt("Value")
            Case "Cold"
                myRating = 0
            Case "Warm"
                myRating = 1
            Case "Hot"
                myRating = 2
        End Select

        sql = "Update DavaoUnifiedPayroll.dbo.IncentiveScopeDetails set Rating = " & myRating & " where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = " & ReadyString(sdt("ID"))

        With Common.WebRequest
            Return .ProData.ExecuteSQL(sql)
        End With

        Return ""
    End Function

    Function saveValue() As Boolean
        Dim sql As String

        sql = "Update DavaoUnifiedPayroll.dbo.IncentiveScopeDetails set EstimatedValue = " & ReadyString(Common.WebRequest.Data("Value")) & " where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = " & ReadyString(Common.WebRequest.Data("ID"))

        With Common.WebRequest
            Return .ProData.ExecuteSQL(sql)
        End With

        Return ""
    End Function

    Function saveClosedDate(ByVal sdt As Dictionary(Of String, Object)) As Boolean
        Dim sql As String

        sql = "Update DavaoUnifiedPayroll.dbo.IncentiveScopeDetails set EstimatedClosedDate = " & ReadyString(sdt("Value")) & " where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = " & ReadyString(sdt("ID"))

        With Common.WebRequest
            Return .ProData.ExecuteSQL(sql)
            Exit Function
        End With

        Return False
    End Function

    Function getCustomerInformation() As String
        Dim sql As String
        Dim dt As New DataTable

        sql = "SELECT dbo.LibraryCustomers.ID, dbo.LibraryCustomers.CompanyName, dbo.LibraryCustomers.ContactNumber, dbo.LibraryBusinessTypes.BusinessType, " &
             "dbo.LibraryCustomersRepresentatives.FirstName + ' ' + dbo.LibraryCustomersRepresentatives.LastName AS ContactPerson, dbo.LibraryCustomers.EmailAddress,  " &
             "dbo.LibraryCustomers.TinNumber,  " &
             "dbo.LibraryCustomersAddresses.StreetNumber + ', ' + dbo.LibraryStreetNames.StreetName + ', ' + dbo.LibraryCities.Name AS Address,  " &
             "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedValue, CONVERT(VARCHAR(10), DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.EstimatedClosedDate, 101) as EstimatedClosedDate, " &
             "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.Rating " &
             "FROM dbo.LibraryCustomers INNER JOIN " &
             "dbo.LibraryBusinessTypes ON dbo.LibraryCustomers.BusinessTypeID = dbo.LibraryBusinessTypes.ID INNER JOIN " &
             "dbo.LibraryCustomersRepresentatives ON dbo.LibraryCustomers.ID = dbo.LibraryCustomersRepresentatives.CustomerID INNER JOIN " &
             "dbo.LibraryCustomersAddresses ON dbo.LibraryCustomers.ID = dbo.LibraryCustomersAddresses.CustomerID INNER JOIN " &
             "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN " &
             "dbo.LibraryCities INNER JOIN dbo.LibraryDistricts ON dbo.LibraryCities.ID = dbo.LibraryDistricts.CityID INNER JOIN " &
             "dbo.LibraryStreetNames ON dbo.LibraryDistricts.ID = dbo.LibraryStreetNames.DistrictID ON  " &
             "dbo.LibraryCustomersAddresses.StreetID = dbo.LibraryStreetNames.ID " &
              "where dbo.LibraryCustomers.ID = " & ReadyString(WebSession.Instance.CustomerID)

        With Common.WebRequest
            dt = .ProData.Execute(sql)
            WebSession.Instance.CustomerName = dt.Rows(0).Item("CompanyName").ToString
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dt)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Function getCustomerNotes() As String
        Dim sql As String
        Dim dt As New DataTable
        Dim strInstruction As String
        Dim ctr As Integer

        sql = "SELECT dbo.LibraryKnowledgeDBase.PostedNote, dbo.LibraryKnowledgeDBase.PostedDatetime, " &
                "DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName AS Author  " &
                "FROM dbo.LibraryKnowledgeDBase INNER JOIN " &
                "DavaoUnifiedPayroll.dbo.EmployeeProfiles ON DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID = dbo.LibraryKnowledgeDBase.UserID " &
                "where CustomerID = " & ReadyString(WebSession.Instance.CustomerID) &
                " order by dbo.LibraryKnowledgeDBase.PostedDatetime desc"

        With Common.WebRequest
            dt = .ProData.Execute(sql)

            While ctr < dt.Rows.Count
                If dt.Rows.Count > 0 Then
                    strInstruction = strInstruction & "<div style='font-size:x-large;'>"
                    strInstruction = strInstruction & "<b>" & Left(dt.Rows(ctr).Item("PostedNote").ToString, 30) & "... </b><br> <br>"
                    strInstruction = strInstruction & "</div>"
                    strInstruction = strInstruction & "<div style='font-size:large;'>"
                    strInstruction = strInstruction & "Date: " & dt.Rows(ctr).Item("PostedDatetime").ToString & "<br> <br>"
                    strInstruction = strInstruction & "Author: " & dt.Rows(ctr).Item("Author").ToString & "<br> <br>"
                    strInstruction = strInstruction & dt.Rows(ctr).Item("PostedNote").ToString
                    strInstruction = strInstruction & "</div> <br/> <br/> <br/>"
                End If
                ctr += 1
            End While

        End With

        'sql = "SELECT ISNULL(LibraryCustomers.Instructions,'') as Instruction1" & _
        '    ",ISNULL(LibraryBusinessTypes.Instructions,'') As Instruction2" & _
        '    " FROM LibraryCustomers" & _
        '    " JOIN LibraryBusinessTypes" & _
        '    " ON LibraryBusinessTypes.ID = LibraryCustomers.BusinessTypeID" & _
        '    " WHERE LibraryCustomers.ID = " & ReadyString(Common.WebSession.CustomerID)

        'With Common.WebRequest
        '    dt = .ProData.Execute(sql)
        '    If dt.Rows.Count > 0 Then
        '        strInstruction += "<br>" & dt.Rows(0).Item("Instruction1").ToString & "<br>" & dt.Rows(0).Item("Instruction2").ToString
        '    End If
        'End With

        Return strInstruction
    End Function

    Function getCustomerName(ByVal ID As String) As String
        Dim sql As String
        Dim dt As New DataTable

        sql = "Select CompanyName from LibraryCustomers " &
             "where ID = " & ReadyString(ID)

        With Common.WebRequest
            dt = .ProData.Execute(sql)
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0).Item("CompanyName").ToString
            End If
        End With
    End Function

    Function getDashBoardData() As String
        Dim dtData As New DataTable
        Dim dtQuota As New DataTable
        Dim dtTotalSales As New DataTable
        Dim Quota() As String
        Dim sql As String
        Dim html As String

        sql = "Select * from DavaoUnifiedPayroll.dbo.IncentiveGroupQuota where DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.TeamID = " & WebSession.Instance.TeamID &
            " and DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = " & Month(Now)

        With Common.WebRequest
            dtQuota = .ProData.Execute(sql)
        End With

        Quota = Split(dtQuota.Rows(0).Item(2).ToString, ",")

        Dim ctrYear As Integer
        Dim dtTemp As New DataTable

        dtTemp.Columns.Add("Data")

        Dim dr As DataRow
        Dim mYear As Integer = Year(Now)

        While ctrYear < 2

            Select Case ctrYear
                Case 0
                    mYear -= 1
                Case 1
                    mYear += 1
            End Select

            sql = "SELECT SUM(dbo.TransInvoice.TotalAmount) AS Total " &
                "FROM dbo.TransInvoice INNER JOIN " &
                "dbo.LibraryCustomers ON dbo.TransInvoice.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON  " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.TransInvoice.CustomerID " &
                "WHERE (MONTH(dbo.TransInvoice.InvoiceDate) = " & Month(Now) & ") AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & mYear & ") " &
                "AND DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID

            With Common.WebRequest
                dtTotalSales = .ProData.Execute(sql)

                dr = dtTemp.NewRow
                dr(0) = dtTotalSales.Rows(0).Item("Total")
                dtTemp.Rows.Add(dr)
            End With

            ctrYear += 1
        End While

        dr = dtTemp.NewRow
        dr(0) = Quota(3)
        dtTemp.Rows.Add(dr)

        dr = dtTemp.NewRow
        dr(0) = Quota(6)
        dtTemp.Rows.Add(dr)

        dr = dtTemp.NewRow
        dr(0) = Quota(9)
        dtTemp.Rows.Add(dr)

        'percent incentive

        dr = dtTemp.NewRow
        dr(0) = Quota(2)
        dtTemp.Rows.Add(dr)

        dr = dtTemp.NewRow
        dr(0) = Quota(5)
        dtTemp.Rows.Add(dr)

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dtTemp)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Function getSalesByMonthly(ByVal Team As String, ByVal Month As String, ByVal Year As String) As String
        Dim dtData As New DataTable
        Dim dtQuota As New DataTable
        Dim dtTotalSales As New DataTable
        Dim dtTotalCollection As New DataTable
        Dim Quota() As String
        Dim sql As String

        If Month = "MMM" Then Exit Function
        If Team = "-Select Value-" Then Team = WebSession.Instance.TeamID

        sql = "Select * from DavaoUnifiedPayroll.dbo.IncentiveGroupQuota where DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.TeamID = " & CInt(Team) &
            " and DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = " & CInt(Month)

        With Common.WebRequest
            dtQuota = .ProData.Execute(sql)
        End With

        Dim dtTemp As New DataTable
        dtTemp.Columns.Add("Data")
        dtTemp.Columns.Add("Value")

        Dim dr As DataRow
        Dim ctrYear As Integer

        If dtQuota.Rows.Count > 0 Then
            Quota = Split(dtQuota.Rows(0).Item(2).ToString, ",")

            While ctrYear < 2

                Select Case ctrYear
                    Case 0
                        Year -= 1
                    Case 1
                        Year += 1
                End Select

                sql = "SELECT SUM(dbo.TransInvoice.TotalAmount) AS Total " &
                "FROM dbo.TransInvoice INNER JOIN " &
                "dbo.LibraryCustomers ON dbo.TransInvoice.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON  " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.TransInvoice.CustomerID " &
                "WHERE (MONTH(dbo.TransInvoice.InvoiceDate) = " & Month & ") AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & Year & ") " &
                "AND DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & IIf(Team Is Nothing, WebSession.Instance.TeamID, Team)

                With Common.WebRequest
                    dtTotalSales = .ProData.Execute(sql)

                    dr = dtTemp.NewRow
                    dr(0) = dtTotalSales.Rows(0).Item("Total")
                    dtTemp.Rows.Add(dr)
                End With

                ctrYear += 1
            End While

            dr = dtTemp.NewRow
            dr(0) = Quota(3)
            dtTemp.Rows.Add(dr)

            dr = dtTemp.NewRow
            dr(0) = Quota(6)
            dtTemp.Rows.Add(dr)

            dr = dtTemp.NewRow
            dr(0) = Quota(9)
            dtTemp.Rows.Add(dr)

            'percent incentive

            dr = dtTemp.NewRow
            dr(0) = Quota(2)
            dtTemp.Rows.Add(dr)

            dr = dtTemp.NewRow
            dr(0) = Quota(5)
            dtTemp.Rows.Add(dr)

            Dim vDt As New List(Of Dictionary(Of String, String))

            vDt = JsonDictionary.TableToDictionary(dtTemp)
            Return JsonDictionary.Serialize(vDt)

        End If
    End Function

    Function getCollection(ByVal Team As String, ByVal Month As String, ByVal Year As String) As String
        'collection period
        Dim dtData As New DataTable
        Dim dtTotalSales As New DataTable
        Dim dtTotalCollection As New DataTable
        Dim dtUncollected As New DataTable
        Dim sql As String
        Dim dtTemp As New DataTable
        Dim dr As DataRow
        Dim ctr As Integer

        Dim l15 As Double
        Dim l30 As Double
        Dim lOthers As Double
        Dim myDue As Integer
        Dim ctrCollection As Integer

        dtTemp.Columns.Add("Data")
        dtTemp.Columns.Add("Value")

        Month = Month - 2
        If Month <= 0 Then
            Month = Month + 12 '12 months
            Year -= 1
        End If

        While ctr < 3
            If Month = 13 Then
                Month = 1
                Year += 1
            End If

            '---------------------------total sales

            sql = "SELECT SUM(dbo.TransInvoice.TotalAmount) AS Total " &
                    "FROM dbo.TransInvoice INNER JOIN " &
                    "dbo.LibraryCustomers ON dbo.TransInvoice.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " &
                    "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON  " &
                    "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.TransInvoice.CustomerID " &
                    "WHERE (MONTH(dbo.TransInvoice.InvoiceDate) = " & Month & ") AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & Year & ") " &
                    "AND DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & IIf(Team Is Nothing, WebSession.Instance.TeamID, Team)

            With Common.WebRequest
                dtTotalSales = .ProData.Execute(sql)

                dr = dtTemp.NewRow
                dr(0) = Month
                dr(1) = dtTotalSales.Rows(0).Item("Total")
                dtTemp.Rows.Add(dr)
            End With

            '"WHERE     (MONTH(dbo.TransCustomerPayment.PaymentDate) = " & CInt(Month) & ") AND (YEAR(dbo.TransCustomerPayment.PaymentDate) = " & CInt(Year) & ") AND (dbo.TransInvoice.TeamID = " & CInt(Team) & ") " & _

            '---------------------------due and collected payments
            sql = "SELECT DISTINCT " & _
                      "TOP (100) PERCENT dbo.TransInvoice.ID, dbo.TransInvoice.InvoiceNumber, dbo.TransInvoiceItems.NetDue, DATEDIFF(day, dbo.TransInvoice.InvoiceDate,  " & _
                      "dbo.TransCustomerPayment.PaymentDate) AS DaysPaid, SUM(dbo.TransInvoiceItems.SubTotal) AS [Group Invoice Amount] " & _
                      "FROM         dbo.TransInvoiceItems INNER JOIN " & _
                      "dbo.TransCustomerPaymentInvoices ON dbo.TransInvoiceItems.InvoiceID = dbo.TransCustomerPaymentInvoices.InvoiceID AND  " & _
                      "dbo.TransInvoiceItems.NetDue = dbo.TransCustomerPaymentInvoices.NetDue INNER JOIN " & _
                      "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID INNER JOIN " & _
                      "dbo.TransCustomerPayment ON dbo.TransCustomerPaymentInvoices.CustomerPaymentID = dbo.TransCustomerPayment.ID LEFT OUTER JOIN " & _
                      "dbo.TransMemoTaxWithheldInvoices ON dbo.TransInvoice.ID = dbo.TransMemoTaxWithheldInvoices.InvoiceID " & _
                      "WHERE     (MONTH(dbo.TransInvoice.InvoiceDate) = " & CInt(Month) & ") AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & CInt(Year) & ") AND (dbo.TransInvoice.TeamID = " & IIf(Team Is Nothing, WebSession.Instance.TeamID, Team) & ") " & _
                      "GROUP BY dbo.TransInvoiceItems.NetDue, dbo.TransCustomerPaymentInvoices.InvoiceAmount, dbo.TransInvoice.ID, dbo.TransInvoice.InvoiceNumber,  " & _
                      "dbo.TransInvoice.InvoiceDate, dbo.TransCustomerPayment.PaymentDate, dbo.TransCustomerPaymentInvoices.AmountPaid, dbo.TransInvoice.PercentIncentive,  " & _
                      "dbo.TransMemoTaxWithheldInvoices.TaxWithheldAmount " & _
                      "HAVING      (SUM(dbo.TransInvoiceItems.SubTotal) - dbo.TransCustomerPaymentInvoices.AmountPaid - ISNULL(dbo.TransMemoTaxWithheldInvoices.TaxWithheldAmount, 0)  " & _
                      "- ISNULL " & _
                      "((SELECT     SUM(TotalAmount) AS Expr1 " & _
                      "FROM         dbo.TransMemoSalesReturn AS TransMemoSalesReturn_1 " & _
                      "WHERE     (InvoiceID = dbo.TransInvoice.ID)), 0) <= 0) " & _
                      "ORDER BY dbo.TransInvoice.InvoiceNumber"

            With Common.WebRequest
                dtTotalCollection = .ProData.Execute(sql)

                If dtTotalCollection.Rows.Count > 0 Then
                    While ctrCollection < dtTotalCollection.Rows.Count
                        myDue = getNetDue(dtTotalCollection.Rows(ctrCollection).Item("NetDue"))


                        If dtTotalCollection.Rows(ctrCollection).Item("DaysPaid") - myDue <= 15 Then
                            l15 += dtTotalCollection.Rows(ctrCollection).Item("Group Invoice Amount")
                        ElseIf dtTotalCollection.Rows(ctrCollection).Item("DaysPaid") - myDue <= 30 Then
                            l30 += dtTotalCollection.Rows(ctrCollection).Item("Group Invoice Amount")
                        Else
                            lOthers += dtTotalCollection.Rows(ctrCollection).Item("Group Invoice Amount")
                        End If

                        ctrCollection += 1
                    End While

                    dr = dtTemp.NewRow
                    dr(0) = "Ohers"
                    dr(1) = IIf(lOthers = 0, 1, lOthers)
                    dtTemp.Rows.Add(dr)

                    dr = dtTemp.NewRow
                    dr(0) = "30"
                    dr(1) = IIf(l30 = 0, 1, l30)
                    dtTemp.Rows.Add(dr)

                    dr = dtTemp.NewRow
                    dr(0) = "15"
                    dr(1) = IIf(l15 = 0, 1, l15)
                    dtTemp.Rows.Add(dr)
                Else
                    dr = dtTemp.NewRow
                    dr(0) = "Ohers"
                    dr(1) = 1
                    dtTemp.Rows.Add(dr)

                    dr = dtTemp.NewRow
                    dr(0) = "30"
                    dr(1) = 1
                    dtTemp.Rows.Add(dr)

                    dr = dtTemp.NewRow
                    dr(0) = "15"
                    dr(1) = 1
                    dtTemp.Rows.Add(dr)
                End If
            End With

            '---------------------- due and uncollected and not yet due query

            sql = "SELECT DISTINCT TOP (100) PERCENT dbo.TransInvoice.InvoiceDate AS [Invoice Date], dbo.TransInvoiceItems.NetDue, " & _
                    "SUM(dbo.TransInvoiceItems.SubTotal) AS [Group Invoice Amount], dbo.TransCustomerPaymentInvoices.AmountPaid " & _
                    "FROM dbo.TransCustomerPayment INNER JOIN " & _
                    "dbo.TransCustomerPaymentInvoices ON dbo.TransCustomerPayment.ID = " & _
                    "dbo.TransCustomerPaymentInvoices.CustomerPaymentID RIGHT OUTER JOIN " & _
                    "dbo.TransInvoiceItems INNER JOIN " & _
                    "dbo.TransInvoice ON dbo.TransInvoiceItems.InvoiceID = dbo.TransInvoice.ID ON " & _
                    "dbo.TransCustomerPaymentInvoices.InvoiceID = dbo.TransInvoiceItems.InvoiceID And " & _
                    "dbo.TransCustomerPaymentInvoices.NetDue = dbo.TransInvoiceItems.NetDue " & _
                    "WHERE (dbo.TransInvoice.TeamID = " & IIf(Team Is Nothing, WebSession.Instance.TeamID, Team) & ") " & _
                    "AND (MONTH(dbo.TransInvoice.InvoiceDate) = " & CInt(Month) & ") " & _
                    "AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & CInt(Year) & ") " & _
                    "AND (dbo.TransCustomerPaymentInvoices.AmountPaid IS NULL) " & _
                    "GROUP BY dbo.TransInvoiceItems.NetDue, dbo.TransCustomerPaymentInvoices.InvoiceAmount, dbo.TransInvoice.ID, " & _
                    "dbo.TransInvoice.InvoiceNumber, dbo.TransInvoice.InvoiceDate, dbo.TransCustomerPayment.PaymentDate, " & _
                    "dbo.TransCustomerPaymentInvoices.AmountPaid, dbo.TransInvoice.PercentIncentive"

            With Common.WebRequest
                dtUncollected = .ProData.Execute(sql)

                If dtUncollected.Rows.Count > 0 Then

                    myDue = 0
                    ctrCollection = 0

                    Dim dueUncollected As Double
                    Dim notDue As Double

                    While ctrCollection < dtUncollected.Rows.Count

                        myDue = getNetDue(dtUncollected.Rows(ctrCollection).Item("NetDue"))

                        If DateDiff(DateInterval.Day, Today, dtUncollected.Rows(ctrCollection).Item("DaysPaid")) - myDue > 0 Then
                            dueUncollected += dtUncollected.Rows(ctrCollection).Item("Group Invoice Amount")
                        Else
                            notDue += dtUncollected.Rows(ctrCollection).Item("Group Invoice Amount")
                        End If

                        ctrCollection += 1
                    End While

                    dr = dtTemp.NewRow
                    dr(0) = "DueUncollected"
                    dr(1) = IIf(dueUncollected = 0, 1, dueUncollected)
                    dtTemp.Rows.Add(dr)

                    dr = dtTemp.NewRow
                    dr(0) = "NotDue"
                    dr(1) = IIf(notDue = 0, 1, notDue)
                    dtTemp.Rows.Add(dr)
                Else
                    dr = dtTemp.NewRow
                    dr(0) = "DueUncollected"
                    dr(1) = 1
                    dtTemp.Rows.Add(dr)

                    dr = dtTemp.NewRow
                    dr(0) = "NotDue"
                    dr(1) = 1
                    dtTemp.Rows.Add(dr)
                End If

            End With

            ctr += 1
            Month += 1
        End While

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dtTemp)
        Return JsonDictionary.Serialize(vDt)
    End Function

    Function getSalesByQuarter(ByVal Quarter As String) As String
        Dim sql As String
        Dim dtQuota As New DataTable

        sql = "SELECT SUM(dbo.TransInvoice.TotalAmount) AS Total " &
            "FROM dbo.TransInvoice INNER JOIN " &
            "dbo.LibraryCustomers ON dbo.TransInvoice.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " &
            "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON  " &
            "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.TransInvoice.CustomerID " &
            "WHERE (MONTH(dbo.TransInvoice.InvoiceDate) = " & Month(Now) & ") AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & Year(Now) & ") " &
            "AND DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID

        Select Case Quarter
            Case "Q1"
                sql += " 1 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 2 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 3)"
            Case "Q2"
                sql += " 4 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 5 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 6)"
            Case "Q3"
                sql += " 7 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 8 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 9)"
            Case Else
                sql += " 10 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 11 or DavaoUnifiedPayroll.dbo.IncentiveGroupQuota.MonthQuota = 12)"
        End Select

        With Common.WebRequest
            dtQuota = .ProData.Execute(sql)
        End With
    End Function

    Function getTeams() As String
        Dim dtTeams As DataTable
        Dim sql As String

        sql = "Select * from DavaoUnifiedPayroll.dbo.IncentiveTeams"

        With Common.WebRequest
            dtTeams = .ProData.Execute(sql)
        End With

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dtTeams)
        Return JsonDictionary.Serialize(vDt)
    End Function

    'Function getTeamInfo(Optional ByVal ID As String = "") As String
    '    Dim dtTeams As DataTable
    '    Dim sql As String

    '    If ID = "" Then ID = 0

    '    sql = "SELECT DavaoUnifiedPayroll.dbo.IncentiveTeams.ID, DavaoUnifiedPayroll.dbo.IncentiveTeams.TeamName, " &
    '            "DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName AS Name " &
    '            "FROM DavaoUnifiedPayroll.dbo.IncentiveTeams INNER JOIN " &
    '            "DavaoUnifiedPayroll.dbo.IncentiveTeamMembers ON " &
    '            "DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = DavaoUnifiedPayroll.dbo.IncentiveTeams.ID INNER JOIN " &
    '            "DavaoUnifiedPayroll.dbo.EmployeeProfiles ON DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " &
    '            "where DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = " & IIf(ID = 0, WebSession.Instance.TeamID, CInt(ID))

    '    With Common.WebRequest
    '        dtTeams = .ProData.Execute(sql)
    '    End With

    '    Dim vDt As New List(Of Dictionary(Of String, String))

    '    vDt = JsonDictionary.TableToDictionary(dtTeams)
    '    Return JsonDictionary.Serialize(vDt)
    'End Function

    Function getTODO(ByVal opt As Integer) As DataTable
        Dim dtTeams As DataTable
        Dim sql As String

        Select Case opt
            Case 0
                sql = "SELECT dbo.TransSalesOrder.ID, dbo.LibraryCustomers.CompanyName, dbo.TransSalesOrder.OrderNo, dbo.LibraryCustomers.ContactNumber, " _
                      & "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID FROM dbo.IncentiveFollowUp INNER JOIN " _
                      & "dbo.TransSalesOrder ON dbo.IncentiveFollowUp.TransactionID = dbo.TransSalesOrder.ID INNER JOIN " _
                      & "dbo.TransStockPickingListItems ON dbo.TransSalesOrder.ID = dbo.TransStockPickingListItems.SalesOrderID INNER JOIN " _
                      & "dbo.TransStockSortingList ON dbo.TransStockPickingListItems.StockPickingListID = dbo.TransStockSortingList.StockPickingListID INNER JOIN " _
                      & "dbo.TransSalesBackOrder ON dbo.TransStockSortingList.ID = dbo.TransSalesBackOrder.StockSortingListID INNER JOIN " _
                      & "dbo.LibraryCustomers ON dbo.TransSalesOrder.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " _
                      & "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON dbo.LibraryCustomers.ID = DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID " _
                      & "where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID
        End Select

        With Common.WebRequest
            dtTeams = .ProData.Execute(sql)
        End With

        Return dtTeams

    End Function

    Function getWheather(ByVal City As String) As String
        Dim inStream As System.IO.StreamReader
        Dim webRequest As System.Net.WebRequest
        Dim webresponse As System.Net.WebResponse

        webRequest = webRequest.Create("http://www.google.com/ig/api?weather=" & Replace(City, " ", "%20"))
        webresponse = webRequest.GetResponse()
        inStream = New System.IO.StreamReader(webresponse.GetResponseStream())

        Dim strContaining() = {"city data=", "condition data=", "temp_f data=", "temp_c data=", "humidity data=", "icon data=", "day_of_week data=", "low data=", "high data=", "icon data="}
        Dim strSplitXML() As String
        Dim strCleanXMLLine As String
        Dim dtWheather As New DataTable

        dtWheather.Columns.Add("Label")
        dtWheather.Columns.Add("Values")

        strCleanXMLLine = inStream.ReadToEnd.Replace(">", "")
        strSplitXML = Split(strCleanXMLLine, "<")

        Dim ctr As Integer
        Dim ctrContaining As Integer

        While ctr < UBound(strSplitXML)
            While ctrContaining < UBound(strContaining)
                If strSplitXML(ctr).Contains(strContaining(ctrContaining)) Then

                    Dim dr As DataRow
                    dr = dtWheather.NewRow

                    'If strSplitXML(ctr).Contains("Overcast") Then
                    '    dr(0) = strContaining(ctrContaining).Replace(" data=", "").Replace("_", " ") & " : "
                    'Else
                    dr(0) = strContaining(ctrContaining).Replace(" data=", "").Replace("_", " ") & " : "
                    'End If


                    If strSplitXML(ctr).Contains(".gif") Then
                        dr(1) = "<img src=http://www.google.com" & Mid(strSplitXML(ctr).Substring(strSplitXML(ctr).IndexOf("""")).Replace("""", ""), 1, strSplitXML(ctr).Substring(strSplitXML(ctr).IndexOf("""")).Replace("""", "").Length - 1) & ">"
                    Else
                        dr(1) = strSplitXML(ctr).Substring(strSplitXML(ctr).IndexOf("""")).Replace("""", "").Replace("/", "")
                    End If

                    dtWheather.Rows.Add(dr)
                End If
                ctrContaining += 1
            End While
            ctrContaining = 0
            ctr += 1
        End While

        Dim vDt As New List(Of Dictionary(Of String, String))

        vDt = JsonDictionary.TableToDictionary(dtWheather)
        Return JsonDictionary.Serialize(vDt)

    End Function

    Private Function getNetDue(ByVal NetDue As String) As Integer
        Select Case NetDue
            Case "30"
                Return 30
            Case "45"
                Return 45
            Case "60"
                Return 60
            Case Else
                Return 0
        End Select
    End Function
#End Region
End Class

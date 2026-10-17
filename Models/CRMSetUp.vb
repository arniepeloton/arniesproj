Public Class CRMSetUp

    Public Function getTeamQuota() As DataTable
        Dim sql As String
        Dim dt As DataTable

        sql = "Select * from davaounifiedpayroll.dbo.IncentiveGroupQuota where davaounifiedpayroll.dbo.IncentiveGroupQuota.TeamID = " & WebSession.Instance.TeamID &
                " Order by davaounifiedpayroll.dbo.IncentiveGroupQuota.MonthQuota"

        With Common.WebRequest
            dt = .ProData.Execute(sql)

        End With

        Return dt
    End Function

    <HttpPost()>
    Public Function getTeamInfo() As DataTable
        Dim pFinder As Finder
        Dim pSql As String
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.Int)
                    .Searchable = False
                End With
                .Add("TeamName", SqlDbType.VarChar)
                .Add("TeamDescription", SqlDbType.VarChar)
                .Add("OfficeLocation", SqlDbType.Int)
                .Add("TeamObjective", SqlDbType.VarChar)
                .Add("DateStarted", SqlDbType.DateTime)
                .Add("isGetSalesFromPOS", SqlDbType.Bit)
                '.Add("Name", SqlDbType.VarChar)
                .Add("QuotaPerMember", SqlDbType.Money)
                .Add("Tier1IncentiveRate", SqlDbType.Decimal)
                .Add("Tier2RequiredGrowth", SqlDbType.Decimal)
                .Add("Tier2IncentiveRate", SqlDbType.Decimal)
                .Add("Tier3RequiredGrowth", SqlDbType.Decimal)
                .Add("Tier3IncentiveRate", SqlDbType.Decimal)
            End With

            .SqlFrom = "FROM DavaoUnifiedPayroll.dbo.IncentiveTeams " 'as T INNER JOIN " &
            '"DavaoUnifiedPayroll.dbo.LibraryBranch as B ON B.ID = T.OfficeLocation "
            .SqlWhere = "where ID = " & WebSession.Instance.TeamID

            AppendToDebug(.CreateSql())
            pDT = .Requery
        End With

        'pDT = Common.WebRequest.ProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            WebSession.Instance.BranchID = pDT.Rows(0).Item("OfficeLocation")
            'WebSession.Instance.BranchName = pDT.Rows(0).Item("Name")
        End If

        Return pDT

    End Function

    Public Function getCustomers(ByVal sdt As Dictionary(Of String, Object)) As DataTable
        Dim sql As String
        Dim dt As DataTable
        Dim strSplit() As String
        Dim ctr As Integer = 1

        strSplit = Split(WebSession.Instance.CustomerName)

        sql = "SELECT DISTINCT dbo.LibraryCustomers.ID, dbo.LibraryCustomers.CompanyName, dbo.LibraryCustomers.UserID, DavaoUnifiedPayroll.dbo.IncentiveTeams.TeamName " &
                "FROM dbo.LibraryCustomers LEFT OUTER JOIN " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID LEFT OUTER JOIN  " &
                "DavaoUnifiedPayroll.dbo.IncentiveTeams ON DavaoUnifiedPayroll.dbo.IncentiveTeams.ID = DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID  " &
                "where dbo.LibraryCustomers.CompanyName like " & ReadyStringWild(strSplit(0))
        'dbo.LibraryCustomers.IsActive = 1 AND 
        While ctr <= UBound(strSplit)
            sql = sql & " AND dbo.LibraryCustomers.CompanyName like " & ReadyStringWild(strSplit(ctr))
            ctr += 1
        End While

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        Return dt
    End Function

    Public Function getTeamCustomers() As DataTable
        Dim sql As String
        Dim dt As DataTable

        sql = "SELECT DISTINCT dbo.LibraryCustomers.ID, dbo.LibraryCustomers.CompanyName, dbo.LibraryCustomers.UserID " &
                "FROM LibraryCustomers INNER JOIN DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = LibraryCustomers.ID " &
                "where TeamID = " & WebSession.Instance.TeamID & " and IsActive = 1 " &
                "Order by dbo.LibraryCustomers.CompanyName"

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        Return dt
    End Function

    <HttpPost()>
    Public Function getTeams(ByVal vkeywords As String) As DataTable
        Dim pFinder As Finder
        Dim pSql As String
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .SearchString = vkeywords

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    '.Expression = "IncentiveTeams.ID"
                    .Searchable = False
                End With
                .Add("TeamName", SqlDbType.VarChar)
                .Add("TeamDescription", SqlDbType.VarChar)

            End With

            .SqlFrom = "From DavaoUnifiedPayroll.dbo.IncentiveTeams"
            pDT = .Requery
        End With

        'pDT = Common.WebRequest.ProData.Execute(pSql)
        Return pDT


        'Dim sql As String
        'Dim dt As DataTable
        'Dim strSplit() As String
        'Dim ctr As Integer = 1

        'strSplit = Split(WebSession.Instance.TeamName, " ")

        'sql = "Select *  where TeamName like " & ReadyStringWild(strSplit(0))

        'While ctr <= UBound(strSplit)
        '    sql = sql & " AND TeamName like " & ReadyStringWild(strSplit(ctr))
        '    ctr += 1
        'End While

        'With Common.WebRequest
        '    dt = .ProData.Execute(sql)
        'End With

        'Return dt
    End Function

    Public Function getBranches(ByVal vkeywords As String) As DataTable
        Dim sql As String
        Dim dt As DataTable
        'not done yet, check query

        Dim pFinder As Finder
        Dim pSql As String
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .SearchString = vkeywords

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                .Add("Code", SqlDbType.VarChar)
                .Add("Name", SqlDbType.VarChar)
                .Add("Address", SqlDbType.VarChar)

            End With

            .SqlFrom = "FROM davaounifiedpayroll.dbo.LibraryBranch"
            pDT = .Requery
        End With

        'pDT = Common.WebRequest.ProData.Execute(pSql)
        Return pDT

        'Dim strSplit() As String
        'Dim ctr As Integer = 1

        'strSplit = Split(WebSession.Instance.BranchName, " ")

        'sql = "Select ID, Code, Name, Address  from davaounifiedpayroll.dbo.LibraryBranch where Name + ' ' + Address like " & ReadyStringWild(strSplit(0))

        'While ctr <= UBound(strSplit)
        '    sql = sql & " AND Name + ' ' + Address like " & ReadyStringWild(strSplit(ctr))
        '    ctr += 1
        'End While

        'With Common.WebRequest
        '    dt = .ProData.Execute(sql)
        'End With

        'Return dt
    End Function

    Public Function RemoveSelectedCustomer()
        Dim sql As String

        sql = "delete from davaounifiedpayroll.dbo.IncentiveScopeDetails where DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = " & ReadyString(WebSession.Instance.CustomerID) &
                " and DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID=" & WebSession.Instance.TeamID

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

    End Function
    Public Function RemoveSelectedEmployee(ByVal sdt As Dictionary(Of String, Object))
        Dim sql As String
        Dim dt As New DataTable

        sql = "Update davaounifiedpayroll.dbo.IncentiveTeamMembers set " &
                "DateEnd = " & ReadyString(sdt("DateEnd")) &
                " Where TeamID = " & WebSession.Instance.TeamID &
                " And EmployeeID = " & WebSession.Instance.EmployeeID

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

    End Function

    Public Function IncentiveCollectionPeriod(ByVal sdt As Dictionary(Of String, Object))
        Dim sql As String
        Dim dt As New DataTable

        sql = "Insert into DavaoUnifiedPayroll.dbo.IncentiveCollectionPeriod(AgingOfAccountsFrom, AgingOfAccountsTo, DecreaseIncentive, Incentive, TeamID, IsSGQ) values(" _
                & ReadyString(WebSession.Instance.Month) & "," & ReadyString(WebSession.Instance.Month & "," & sdt("RangeQuota")) & ", 1" & "," & ReadyString(WebSession.Instance.TeamID) & ")"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With
    End Function

    Public Function saveQuotaInfo(ByVal sdt As Dictionary(Of String, Object))
        Dim sql As String
        Dim dt As New DataTable

        sql = "delete from DavaoUnifiedPayroll.dbo.IncentiveGroupQuota where TeamID = " & WebSession.Instance.TeamID &
                " AND MonthQuota = " & ReadyString(WebSession.Instance.Month)

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

        sql = "Insert into DavaoUnifiedPayroll.dbo.IncentiveGroupQuota(MonthQuota, RangeQuota, isMonthly, TeamID) values(" _
                & ReadyString(WebSession.Instance.Month) & "," & ReadyString(WebSession.Instance.Month & "," & sdt("RangeQuota")) & ", 1" & "," & ReadyString(WebSession.Instance.TeamID) & ")"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With
    End Function

    Public Function saveItemIncentive(ByVal sdt As Dictionary(Of String, Object))
        Dim sql As String
        Dim dt As New DataTable

        If WebSession.Instance.IncentiveItemID = "undefined" OrElse WebSession.Instance.IncentiveItemID = vbNullString Then
            sql = "Insert into davaounifiedpayroll.dbo.IncentiveProductQuota(ProductID, CommissionType, Quota, Incentive, TeamID, UnitMeasureID) values(" &
                ReadyString(sdt("skuID")) & "," &
                1 & "," &
                sdt("Qty") & "," &
                sdt("Incentive") & "," &
                WebSession.Instance.TeamID & "," &
                ReadyString(sdt("UnitMeasure")) & ")"
        Else
            sql = "Update davaounifiedpayroll.dbo.IncentiveProductQuota set " &
                "ProductID = " & ReadyString(sdt("skuID")) & "," &
                "CommissionType=1, " &
                "Quota = " & sdt("Qty") & "," &
                "Incentive = " & sdt("Incentive") & "," &
                "TeamID = " & WebSession.Instance.TeamID & "," &
                "UnitMeasureID = " & ReadyString(sdt("UnitMeasure")) &
                " Where ID = " & ReadyString(WebSession.Instance.IncentiveItemID)
        End If

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
            WebSession.Instance.IncentiveItemID = vbNullString
        End With
    End Function

    Public Function saveSalesQuotaPerMember()

        Dim p As New InsertUpdate("DavaoUnifiedPayroll.dbo.IncentiveTeams")
        Dim pSql As String


        p("QuotaPerMember") = Common.WebRequest.Data("QuotaPerMember")
        p("Tier1IncentiveRate") = Common.WebRequest.Data("Tier1IncentiveRate")
        p("Tier2RequiredGrowth") = Common.WebRequest.Data("Tier2RequiredGrowth")
        p("Tier2IncentiveRate") = Common.WebRequest.Data("Tier2IncentiveRate")
        p("Tier3RequiredGrowth") = Common.WebRequest.Data("Tier3RequiredGrowth")
        p("Tier3IncentiveRate") = Common.WebRequest.Data("Tier3IncentiveRate")

        pSql = p.GetUpdate("DavaoUnifiedPayroll.dbo.IncentiveTeams.ID = " & WebSession.Instance.TeamID)

        Common.WebRequest.ProData.ExecuteSQL(pSql)

    End Function

    Public Function saveTeamLeadersCut(ByVal sdt As Dictionary(Of String, Object)) As String
        Dim sql As String
        Dim dt As New DataTable

        sql = "delete from davaounifiedpayroll.dbo.IncentiveFinancialRewardsDetails where davaounifiedpayroll.dbo.IncentiveFinancialRewardsDetails.TeamID = " & WebSession.Instance.TeamID

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

        sql = "Insert into davaounifiedpayroll.dbo.IncentiveFinancialRewardsDetails (FinancialRewardsID, OutrightShare, TeamID) values(" &
                "2, " &
                sdt("OutrightShare") & "," &
                WebSession.Instance.TeamID & ")"
        
        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

    End Function

    Public Function saveTeam() As String
        Dim p As New InsertUpdate("DavaoUnifiedPayroll.dbo.IncentiveTeams")
        Dim pSql As String


        p("TeamName") = ReadyString(Common.WebRequest.Data("TeamName"))
        p("TeamDescription") = ReadyString(Common.WebRequest.Data("Description"))
        p("OfficeLocation") = WebSession.Instance.TeamID
        p("TeamObjective") = ReadyString(Common.WebRequest.Data("Objective"))

        If WebSession.Instance.IsNew = 0 Then
            pSql = p.GetUpdate("DavaoUnifiedPayroll.dbo.IncentiveTeams.ID = " & WebSession.Instance.TeamID)
        Else
            pSql = p.GetInsert
        End If

        Common.WebRequest.ProData.ExecuteSQL(pSql)

        If WebSession.Instance.IsNew = 1 Then
            Dim pFinder As New Finder
            Dim pDT As DataTable

            With pFinder
                .ProData = Common.WebRequest.ProData

                .MaxRecords = 1

                With .Fields
                    With .Add("ID", SqlDbType.Int)
                    End With
                End With

                .SqlFrom = "from DavaoUnifiedPayroll.dbo.IncentiveTeams"
                .SqlOrder = "ORDER BY ID desc"
                pDT = .Requery
            End With

            If pDT.Rows.Count > 0 Then
                WebSession.Instance.TeamID = pDT.Rows(0).Item("ID")
            End If

            'Sql = "Select top 1 ID from DavaoUnifiedPayroll.dbo.IncentiveTeams order by ID desc"

            'With Common.WebRequest
            '    dt = .ProData.Execute(Sql)
            '    If dt.Rows.Count > 0 Then
            '        WebSession.Instance.TeamID = dt.Rows(0).Item("ID")
            '    End If
            'End With
        End If

    End Function

    Public Function getCustomerInfo() As DataTable
        Dim sql As String
        Dim dt As New DataTable

        sql = "SELECT DISTINCT " &
            "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.ID, dbo.LibraryCustomers.CompanyName, DavaoUnifiedPayroll.dbo.IncentiveTeams.TeamName, " &
            "DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.MiddleName AS Name, DavaoUnifiedPayroll.dbo.EmployeeProfiles.EmailAddress " &
            "FROM DavaoUnifiedPayroll.dbo.IncentiveScopeDetails INNER JOIN DavaoUnifiedPayroll.dbo.IncentiveTeams ON DavaoUnifiedPayroll.dbo.IncentiveTeams.ID = DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID INNER JOIN " &
            "dbo.LibraryCustomers ON DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.LibraryCustomers.ID INNER JOIN DavaoUnifiedPayroll.dbo.IncentiveTeamMembers ON  " &
            "DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = DavaoUnifiedPayroll.dbo.IncentiveTeams.ID INNER JOIN " &
            "DavaoUnifiedPayroll.dbo.EmployeeProfiles ON DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " &
            "Where dbo.LibraryCustomers.ID = " & ReadyString(WebSession.Instance.CustomerID)

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        Return dt
    End Function

    Public Function getItems() As DataTable
        Dim sql As String
        Dim dt As DataTable
        Dim strSplit() As String
        Dim ctr

        strSplit = Split(WebSession.Instance.IncentiveItemName, " ")

        sql = "SELECT dbo.LibraryStockInformation.ID, dbo.LibraryStockInformation.StockNumber, ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ' ,'), '')  " &
            "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ' ,'), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') + dbo.LibraryStockInformation.UnitSize " &
            "AS [Stock Description] " &
            "FROM dbo.LibraryStockInformation INNER JOIN " &
            "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
            "dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID " &
            "WHERE  (dbo.LibraryStockInformation.IsActive = 1)"

        While ctr <= UBound(strSplit)
            sql = sql & " AND ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ' ,'), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ' ,'), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') + dbo.LibraryStockInformation.UnitSize LIKE " &
                ReadyStringWild(strSplit(ctr))
            ctr += 1
        End While

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        Return dt
    End Function

    Public Function getItemIncentiveDetail() As DataTable
        Dim sql As String
        Dim dt As DataTable

        If Not WebSession.Instance.SkuID = vbNullString Then
            sql = "SELECT dbo.LibraryStockInformation.ID as ProductID, ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ' ,'), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ' ,'), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') + dbo.LibraryStockInformation.UnitSize " &
                "AS [Stock Description], dbo.LibraryUnitMeasures.ID, dbo.LibraryUnitMeasures.UnitMeasure, dbo.LibraryUnitMeasures.ID as UnitMeasureID " &
                "FROM dbo.LibraryUnitMeasures INNER JOIN dbo.LibraryStockMeasurements ON dbo.LibraryUnitMeasures.ID = dbo.LibraryStockMeasurements.UnitMeasureID INNER JOIN " &
                "dbo.LibraryStockInformation INNER JOIN dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
                "dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID ON dbo.LibraryStockMeasurements.StockID = dbo.LibraryStockInformation.ID " &
                "WHERE (dbo.LibraryStockInformation.IsActive = 1) AND dbo.LibraryStockInformation.ID = " & ReadyString(WebSession.Instance.SkuID) &
                " Order by BrandName, ProductName, dbo.LibraryStockMeasurements.MeasurementLevel desc"
        Else
            sql = "SELECT     DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ID, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ProductID, " &
                    "DavaoUnifiedPayroll.dbo.IncentiveProductQuota.CommissionType, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.Quota, " &
                    "DavaoUnifiedPayroll.dbo.IncentiveProductQuota.Incentive, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.TeamID,  " &
                    "DavaoUnifiedPayroll.dbo.IncentiveProductQuota.UnitMeasureID, ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '')  " &
                    "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ' ,'), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ' ,'), '')  + dbo.LibraryStockInformation.UnitSize " &
                    "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '') AS [Stock Description], dbo.LibraryUnitMeasures.UnitMeasure " &
                    "FROM DavaoUnifiedPayroll.dbo.IncentiveProductQuota INNER JOIN " &
                    "dbo.LibraryStockInformation ON DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ProductID = dbo.LibraryStockInformation.ID INNER JOIN " &
                    "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
                    "dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID INNER JOIN " &
                    "dbo.LibraryUnitMeasures ON DavaoUnifiedPayroll.dbo.IncentiveProductQuota.UnitMeasureID = dbo.LibraryUnitMeasures.ID " &
                "WHERE DavaoUnifiedPayroll.dbo.IncentiveProductQuota.TeamID = " & WebSession.Instance.TeamID &
                " AND DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ID = " & ReadyString(WebSession.Instance.IncentiveItemID) &
                " AND dbo.LibraryStockInformation.IsActive = 1"
        End If

        With Common.WebRequest
            dt = .ProData.Execute(sql)
            WebSession.Instance.SkuID = vbNullString
        End With

        Return dt
    End Function

    Public Function CheckIfCustomerExist() As String
        Dim sql As String
        Dim dt As New DataTable

        sql = "Select * from davaounifiedpayroll.dbo.IncentiveScopeDetails where CustomerID = " & ReadyString(WebSession.Instance.CustomerID)

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        If dt.Rows.Count = 0 Then
            AddCustomers()
            Return "0"
        Else
            Return "1"
        End If
    End Function

    Public Function AddCustomers()
        Dim sql As String
        Dim dt As New DataTable

        sql = "Insert into davaounifiedpayroll.dbo.IncentiveScopeDetails(CustomerID, TeamID) values(" &
                ReadyString(WebSession.Instance.CustomerID) & "," &
                WebSession.Instance.TeamID & ")"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

    End Function

    Public Function assignAsTeamLeader(ByVal EmpID As String)
        Dim sql As String
        Dim dt As New DataTable

        Dim p As New InsertUpdate("davaounifiedpayroll.dbo.IncentiveTeamMembers")
        Dim pSql As String

        p("IsLeader") = 0
        pSql = p.GetUpdate("TeamID = " & WebSession.Instance.TeamID)
        Common.WebRequest.ProData.ExecuteSQL(pSql)


        p = New InsertUpdate("davaounifiedpayroll.dbo.IncentiveTeamMembers")
        p("IsLeader") = 1
        pSql = p.GetUpdate("TeamID = " & WebSession.Instance.TeamID & " AND davaounifiedpayroll.dbo.IncentiveTeamMembers.EmployeeID = " & EmpID)
        Common.WebRequest.ProData.ExecuteSQL(pSql)

        'Common.WebRequest.Data("m_BranchName")

        'sql = "Update davaounifiedpayroll.dbo.IncentiveTeamMembers set " &
        '        "IsLeader = 0 " &
        '        " Where TeamID = " & WebSession.Instance.TeamID
        'With Common.WebRequest
        '    .ProData.ExecuteSQL(sql)
        'End With

        'sql = "Update davaounifiedpayroll.dbo.IncentiveTeamMembers set " &
        '        "IsLeader = 1 " &
        '        " Where TeamID = " & WebSession.Instance.TeamID &
        '        " AND davaounifiedpayroll.dbo.IncentiveTeamMembers.EmployeeID = " & sdt("ID")
        'With Common.WebRequest
        '    .ProData.ExecuteSQL(sql)
        'End With

    End Function

    Public Function UpdateEmployee(ByVal sdt As Dictionary(Of String, Object))
        Dim sql As String
        Dim dt As New DataTable

        sql = "Update davaounifiedpayroll.dbo.IncentiveTeamMembers set " &
                "DateStart = " & ReadyString(sdt("DateStart")) &
                " Where TeamID = " & WebSession.Instance.TeamID &
                " And EmployeeID = " & WebSession.Instance.EmployeeID

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With
    End Function

    Public Function AddEmployee(ByVal sdt As Dictionary(Of String, Object)) As String
        Dim sql As String
        Dim dt As New DataTable

        sql = "Insert into davaounifiedpayroll.dbo.IncentiveTeamMembers(TeamID, EmployeeID) values(" &
                ReadyString(WebSession.Instance.TeamID) & "," &
                ReadyString(sdt("EmployeeID")) & ")"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

        Return sdt("EmployeeID")
    End Function

    Public Function GetTeamMembers(ByVal vkeywords As String) As DataTable
        Dim pFinder As Finder
        Dim pSql As String
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID"
                    .Searchable = False
                End With

                With .Add("Name", SqlDbType.VarChar)
                    .Expression = "DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName  + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.MiddleName"
                End With

                .Add("IsLeader", SqlDbType.Bit)

            End With

            .SqlFrom = "from DavaoUnifiedPayroll.dbo.IncentiveTeamMembers " &
              "inner join DavaoUnifiedPayroll.dbo.EmployeeProfiles on DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID "
            .SqlWhere = "where DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = " & WebSession.Instance.TeamID & " and DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.DateEnd IS NULL"

            .SqlOrder = "Order by DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.LastUpdated desc"

            pDT = .Requery
        End With

        Return pDT

    End Function

    Public Function getItemIncentive() As DataTable
        Dim sql As String
        Dim dt As DataTable

        sql = "SELECT DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ID, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ProductID, " &
                "DavaoUnifiedPayroll.dbo.IncentiveProductQuota.CommissionType, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.Quota, " &
                "DavaoUnifiedPayroll.dbo.IncentiveProductQuota.Incentive, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.TeamID,  " &
                "DavaoUnifiedPayroll.dbo.IncentiveProductQuota.UnitMeasureID, DavaoUnifiedPayroll.dbo.IncentiveProductQuota.UnitMeasureBase,  " &
                "ISNULL(NULLIF (LTRIM(RTRIM(dbo.LibraryBrands.BrandName)), ''), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryProducts.ProductName)), ' ,'), '')  " &
                "+ ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.Make)), ' ,'), '') + ISNULL(NULLIF (', ' + LTRIM(RTRIM(dbo.LibraryStockInformation.[Content])), ', '), '')  + dbo.LibraryStockInformation.UnitSize " &
                "AS [Stock Description] " &
                "FROM         DavaoUnifiedPayroll.dbo.IncentiveProductQuota INNER JOIN " &
                "dbo.LibraryStockInformation ON DavaoUnifiedPayroll.dbo.IncentiveProductQuota.ProductID = dbo.LibraryStockInformation.ID INNER JOIN " &
                "dbo.LibraryProducts ON dbo.LibraryStockInformation.ProductID = dbo.LibraryProducts.ID INNER JOIN " &
                "dbo.LibraryBrands ON dbo.LibraryStockInformation.BrandID = dbo.LibraryBrands.ID " &
                "WHERE DavaoUnifiedPayroll.dbo.IncentiveProductQuota.TeamID = " & WebSession.Instance.TeamID & " AND dbo.LibraryStockInformation.IsActive = 1"

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        Return dt
    End Function

    Public Function getEmployees(ByVal sdt As Dictionary(Of String, Object)) As DataTable
        Dim sql As String
        Dim dt As DataTable
        Dim dtQuota As New DataTable
        Dim strSplit() As String
        Dim ctr As Integer = 1

        If Not sdt.Count = 0 Then
            strSplit = Split(WebSession.Instance.FullName, " ")

            sql = "Select DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID, DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + " &
                "DavaoUnifiedPayroll.dbo.EmployeeProfiles.MiddleName as Name " &
                "from DavaoUnifiedPayroll.dbo.EmployeeProfiles " &
                "where DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID NOT IN (Select DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " &
                "from DavaoUnifiedPayroll.dbo.IncentiveTeamMembers " &
                "inner join DavaoUnifiedPayroll.dbo.EmployeeProfiles on DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " &
                "where DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = " & WebSession.Instance.TeamID & ") " &
                "AND DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName like " & ReadyStringWild(strSplit(0))

            While ctr <= UBound(strSplit)
                sql += " AND DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName like " & ReadyStringWild(strSplit(ctr))
                ctr += 1
            End While
        Else
            sql = "Select DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID, DavaoUnifiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUnifiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + " &
                "DavaoUnifiedPayroll.dbo.EmployeeProfiles.MiddleName as Name " &
                "from DavaoUnifiedPayroll.dbo.EmployeeProfiles inner join DavaoUnifiedPayroll.dbo.IncentiveTeamMembers " &
                "ON DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID " &
                "where DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = " & WebSession.Instance.TeamID &
                "AND DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID = " & WebSession.Instance.EmployeeID
        End If

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        Return dt
    End Function

    Public Function getTeamLeadersCut() As DataTable
        Dim sql As String
        Dim dt As DataTable
    
        sql = "Select * from davaounifiedpayroll.dbo.IncentiveFinancialRewardsDetails where davaounifiedpayroll.dbo.IncentiveFinancialRewardsDetails.TeamID = " & WebSession.Instance.TeamID

        With Common.WebRequest
            dt = .ProData.Execute(sql)
            Return dt
        End With
    End Function

    Public Function GetQuotaInfo() As DataTable
        Dim pFinder As Finder
        Dim pSql As String
        Dim pDT As DataTable

        Dim dtQuota As New DataTable
        Dim strsplit() As String
        Dim dr As DataRow

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                
                .Add("ID", SqlDbType.UniqueIdentifier)
                .Add("MonthQuota", SqlDbType.VarChar)
                .Add("RangeQuota", SqlDbType.VarChar)

            End With

            .SqlFrom = "FROM davaounifiedpayroll.dbo.IncentiveGroupQuota"
            .SqlWhere = "where davaounifiedpayroll.dbo.IncentiveGroupQuota.TeamID = " & WebSession.Instance.TeamID & " and davaounifiedpayroll.dbo.IncentiveGroupQuota.MonthQuota = " & WebSession.Instance.Month

            pDT = .Requery
        End With

        If pDT.Rows.Count > 0 Then
            dtQuota.Columns.Add("From")
            dtQuota.Columns.Add("To")
            dtQuota.Columns.Add("Incentive")
            strsplit = Split(pDT.Rows(0).Item("RangeQuota"), ",")

            dr = dtQuota.NewRow
            dr(0) = strsplit(1)
            dr(2) = strsplit(2)
            dtQuota.Rows.Add(dr)

            If strsplit.Length > 3 Then
                dr = dtQuota.NewRow
                dr(0) = strsplit(3)
                dr(1) = strsplit(4)
                dr(2) = strsplit(5)
                dtQuota.Rows.Add(dr)
            End If

            If strsplit.Length > 5 Then
                dr = dtQuota.NewRow
                dr(0) = strsplit(6)
                dr(1) = strsplit(7)
                dr(2) = strsplit(8)
                dtQuota.Rows.Add(dr)
            End If

            If strsplit.Length > 8 Then
                dr = dtQuota.NewRow
                dr(0) = strsplit(9)
                dr(1) = strsplit(10)
                dr(2) = strsplit(11)
                dtQuota.Rows.Add(dr)
            End If

            If strsplit.Length > 11 Then
                dr = dtQuota.NewRow
                dr(0) = strsplit(12)
                dr(1) = strsplit(13)
                dtQuota.Rows.Add(dr)
            End If
        End If

        Return dtQuota


        'Dim sql As String
        'Dim dt As DataTable
        'Dim dtQuota As New DataTable
        'Dim strsplit() As String
        'Dim dr As DataRow

        'MonthQuota()
        'RangeQuota()

        ''sql = "Select * from davaounifiedpayroll.dbo.IncentiveGroupQuota where davaounifiedpayroll.dbo.IncentiveGroupQuota.TeamID = " & WebSession.Instance.TeamID & " and davaounifiedpayroll.dbo.IncentiveGroupQuota.MonthQuota = " & WebSession.Instance.Month


        'With Common.WebRequest
        '    dt = .ProData.Execute(sql)

        '    dtQuota.Columns.Add("From")
        '    dtQuota.Columns.Add("To")
        '    dtQuota.Columns.Add("Incentive")

        '    If dt.Rows.Count > 0 Then
        '        strsplit = Split(dt.Rows(0).Item("RangeQuota"), ",")

        '        dr = dtQuota.NewRow
        '        dr(0) = strsplit(1)
        '        dr(2) = strsplit(2)
        '        dtQuota.Rows.Add(dr)

        '        If strsplit.Length > 3 Then
        '            dr = dtQuota.NewRow
        '            dr(0) = strsplit(3)
        '            dr(1) = strsplit(4)
        '            dr(2) = strsplit(5)
        '            dtQuota.Rows.Add(dr)
        '        End If

        '        If strsplit.Length > 5 Then
        '            dr = dtQuota.NewRow
        '            dr(0) = strsplit(6)
        '            dr(1) = strsplit(7)
        '            dr(2) = strsplit(8)
        '            dtQuota.Rows.Add(dr)
        '        End If

        '        If strsplit.Length > 8 Then
        '            dr = dtQuota.NewRow
        '            dr(0) = strsplit(9)
        '            dr(1) = strsplit(10)
        '            dr(2) = strsplit(11)
        '            dtQuota.Rows.Add(dr)
        '        End If

        '        If strsplit.Length > 11 Then
        '            dr = dtQuota.NewRow
        '            dr(0) = strsplit(12)
        '            dr(1) = strsplit(13)
        '            dtQuota.Rows.Add(dr)
        '        End If
        '    End If
        'End With

        'Return dtQuota
    End Function

    Public Function GetCollectionInfo() As DataTable
        Dim sql As String
        Dim dt As DataTable
        Dim dtCollection As New DataTable
        Dim strsplitFrom() As String
        Dim strsplitTo() As String
        Dim strsplitPercent() As String
        Dim dr As DataRow

        sql = "Select * from davaounifiedpayroll.dbo.IncentiveCollectionPeriod where davaounifiedpayroll.dbo.IncentiveCollectionPeriod.TeamID = " & WebSession.Instance.TeamID

        With Common.WebRequest
            dt = .ProData.Execute(sql)

            dtCollection.Columns.Add("From")
            dtCollection.Columns.Add("To")
            dtCollection.Columns.Add("Incentive")

            If dt.Rows.Count > 0 Then
                strsplitFrom = Split(dt.Rows(0).Item("AgingOfAccountsFrom"), ",")
                strsplitTo = Split(dt.Rows(0).Item("AgingOfAccountsTo"), ",")
                strsplitPercent = Split(dt.Rows(0).Item("DecreaseIncentive"), ",")

                dr = dtCollection.NewRow
                dr(0) = strsplitFrom(0)
                dr(2) = strsplitPercent(0)
                dtCollection.Rows.Add(dr)

                dr = dtCollection.NewRow
                dr(0) = strsplitFrom(1)
                dr(1) = strsplitTo(0)
                dr(2) = strsplitPercent(1)
                dtCollection.Rows.Add(dr)

                dr = dtCollection.NewRow
                dr(0) = strsplitFrom(2)
                dr(1) = strsplitTo(1)
                dr(2) = strsplitPercent(2)
                dtCollection.Rows.Add(dr)

                dr = dtCollection.NewRow
                dr(0) = strsplitFrom(3)
                dr(2) = strsplitPercent(3)
                dtCollection.Rows.Add(dr)

            End If
        End With

        Return dtCollection
    End Function

    Public Function saveCollectionInfo(ByVal sdt As Dictionary(Of String, Object))
        Dim sql As String
        Dim dt As New DataTable

        sql = "delete from DavaoUnifiedPayroll.dbo.IncentiveCollectionPeriod where TeamID = " & WebSession.Instance.TeamID

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With

        sql = "Insert into DavaoUnifiedPayroll.dbo.IncentiveCollectionPeriod(AgingOfAccountsFrom, AgingOfAccountsTo, DecreaseIncentive, TeamID) values(" _
                & ReadyString(sdt("AgingOfAccountsFrom")) & "," & ReadyString(sdt("AgingOfAccountsTo")) & "," & ReadyString(sdt("DecreaseIncentive")) & "," & ReadyString(WebSession.Instance.TeamID) & ")"

        With Common.WebRequest
            .ProData.ExecuteSQL(sql)
        End With
    End Function

    Function ReCalculateBaseQuota() As String
        Dim CurrentQuota As DataTable
        Dim dtQuotaPerMember As DataTable
        Dim NumberOfMembers As Integer
        Dim dt As DataTable
        Dim sql As String
        Dim LastYearQuota As Double
        Dim Last2YearsQuota As Double
        Dim Last3YearsQuota As Double
        Dim mYear As Integer = Year(Now)
        Dim NewQuota As Double
        Dim QuotaPerMember As Double
        Dim ctr As Integer = 1

        sql = "Select count(*) as Count from DavaoUnifiedPayroll.dbo.IncentiveTeamMembers " &
                    "inner join DavaoUnifiedPayroll.dbo.EmployeeProfiles on DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.EmployeeID = DavaoUnifiedPayroll.dbo.EmployeeProfiles.ID where DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.TeamID = " & WebSession.Instance.TeamID &
                    " and DavaoUnifiedPayroll.dbo.IncentiveTeamMembers.DateEnd IS NULL "

        With Common.WebRequest
            dt = .ProData.Execute(sql)
            NumberOfMembers = dt.Rows(0).Item("Count")
        End With

        dtQuotaPerMember = getTeamInfo()

        QuotaPerMember = NumberOfMembers * dtQuotaPerMember.Rows(0).Item("QuotaPerMember")

        While ctr <= 12

            Dim lstGetMax As New List(Of Double)

            WebSession.Instance.Month = ctr

            CurrentQuota = GetQuotaInfo()

            LastYearQuota = getOldQuota(mYear - 1, ctr)
            Last2YearsQuota = getOldQuota(mYear - 2, ctr)
            Last3YearsQuota = getOldQuota(mYear - 3, ctr)

            lstGetMax.Add(QuotaPerMember)
            lstGetMax.Add(LastYearQuota)
            lstGetMax.Add(Last2YearsQuota)
            lstGetMax.Add(Last3YearsQuota)

            NewQuota = lstGetMax.Max

            Dim strQuota As String
            Dim Total As Double

            strQuota = NewQuota & ","

            If CurrentQuota.Rows.Count > 4 Then
                If Not CurrentQuota.Rows(4).Item("From") = vbNullString Then
                    strQuota += CurrentQuota.Rows(0).Item("Incentive") & ","

                    Total = NewQuota + (NewQuota * CurrentQuota.Rows(4).Item("From") / 100)

                    strQuota += NewQuota & "," & Total & "," & CurrentQuota.Rows(1).Item("Incentive") & "," & Total & ","

                    If Not CurrentQuota.Rows(4).Item("To") = 0 Then

                        Total = NewQuota + (NewQuota * CurrentQuota.Rows(4).Item("To") / 100)

                        strQuota += Total & "," & CurrentQuota.Rows(2).Item("Incentive") & "," & Total & ",10000000," & CurrentQuota.Rows(3).Item("Incentive") & ","
                    End If

                    strQuota += CurrentQuota.Rows(4).Item("From") & "," & CurrentQuota.Rows(4).Item("To")
                Else
                    strQuota = CalculateDefaultQuota(NewQuota, dtQuotaPerMember)
                End If
            Else
                strQuota = CalculateDefaultQuota(NewQuota, dtQuotaPerMember)
            End If

            sql = "delete from DavaoUnifiedPayroll.dbo.IncentiveGroupQuota where TeamID = " & WebSession.Instance.TeamID &
                    " AND MonthQuota = " & ctr

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

            sql = "Insert into DavaoUnifiedPayroll.dbo.IncentiveGroupQuota(MonthQuota, RangeQuota, isMonthly, TeamID) values(" _
                    & ReadyString(ctr) & "," & ReadyString(WebSession.Instance.Month & "," & strQuota) & ", 1" & "," & ReadyString(WebSession.Instance.TeamID) & ")"

            With Common.WebRequest
                .ProData.ExecuteSQL(sql)
            End With

            ctr += 1
        End While

        Return ""
    End Function

    Function CalculateDefaultQuota(ByVal NewQuota As Double, ByVal dtQuotaPerMember As DataTable) As String
        Dim strQuota As String
        Dim Total As Double

        strQuota = NewQuota & "," & dtQuotaPerMember.Rows(0).Item("Tier1IncentiveRate") & ","

        Total = NewQuota + (NewQuota * dtQuotaPerMember.Rows(0).Item("Tier2RequiredGrowth") / 100)

        strQuota += NewQuota & "," & Total & "," & dtQuotaPerMember.Rows(0).Item("Tier1IncentiveRate") & "," & Total & ","


        Total = NewQuota + (NewQuota * dtQuotaPerMember.Rows(0).Item("Tier3RequiredGrowth") / 100)

        strQuota += Total & "," & dtQuotaPerMember.Rows(0).Item("Tier2IncentiveRate") & "," & Total & ",10000000," & dtQuotaPerMember.Rows(0).Item("Tier3IncentiveRate") & ","


        strQuota += dtQuotaPerMember.Rows(0).Item("Tier2RequiredGrowth") & "," & dtQuotaPerMember.Rows(0).Item("Tier3RequiredGrowth")

        Return strQuota
    End Function

    Function getOldQuota(ByVal mYear As Integer, ByVal mMonth As Integer) As Double
        Dim sql As String
        Dim dt As DataTable

        sql = "SELECT SUM(dbo.TransInvoice.TotalAmount) AS Total " &
                "FROM dbo.TransInvoice INNER JOIN " &
                "dbo.LibraryCustomers ON dbo.TransInvoice.CustomerID = dbo.LibraryCustomers.ID INNER JOIN " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails ON  " &
                "DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.CustomerID = dbo.TransInvoice.CustomerID " &
                "WHERE (MONTH(dbo.TransInvoice.InvoiceDate) = " & mMonth & ") AND (YEAR(dbo.TransInvoice.InvoiceDate) = " & mYear & ") " &
                "AND DavaoUnifiedPayroll.dbo.IncentiveScopeDetails.TeamID = " & WebSession.Instance.TeamID

        With Common.WebRequest
            dt = .ProData.Execute(sql)
        End With

        If dt.Rows(0).Item("Total").ToString = vbNullString Then
            Return 0
        Else
            Return dt.Rows(0).Item("Total")
        End If

    End Function
End Class

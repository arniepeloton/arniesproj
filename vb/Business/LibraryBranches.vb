Public Class LibraryBranches

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function Schedules(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        ' 2012 coz 1st day of the 1st month of 2012 is the 1st day of the week (sunday)
        pSql = "SELECT ID" & _
            ",CompilationDay" & _
            ",datename(dw, '1/' + CONVERT(varchar, CompilationDay) + '/2012') AS CompilationDayName" & _
            ",CompilationTime" & _
            ",SubmissionDay" & _
            ",datename(dw, '1/' + CONVERT(varchar, SubmissionDay) + '/2012') AS SubmissionDayName" & _
            ",SubmissionTime" & _
            " FROM LibraryBranchSchedules" & _
            " WHERE CONVERT(varchar(36), BranchID) = " & ReadyString(vID) & _
            " ORDER BY CompilationDay, CompilationTime"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function CurrentSchedule(ByVal vID As String) As DataView

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pDRV As DataRowView
        Dim p As Integer
        Dim pNow As Date

        pDT = Schedules(vID)
        pNow = Now
        pDT.Columns.Add("DateFrom", GetType(Date))
        pDT.Columns.Add("DateTo", GetType(Date))
        pDT.Columns.Add("SubmissionDate", GetType(Date))

        For Each pDR In pDT.Rows
            pDR("SubmissionDate") = pNow.AddDays(1 - Weekday(pNow)).Date  ' the Sunday of the current week
            pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(pDR("SubmissionDay") - 1) ' add the compilation day
            pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).Add(CDate(pDR("SubmissionTime")).TimeOfDay)   ' add the compilation time
            'If pDR("SubmissionDate") < pDR("DateTo") Then pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(7)  ' original code
            If pDR("SubmissionDate") < pNow Then pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(7)

            pDR("DateTo") = pNow.AddDays(1 - Weekday(pNow)).Date  ' the Sunday of the current week
            pDR("DateTo") = CDate(pDR("DateTo")).AddDays(pDR("CompilationDay") - 1) ' add the compilation day
            pDR("DateTo") = CDate(pDR("DateTo")).Add(CDate(pDR("CompilationTime")).TimeOfDay)   ' add the compilation time
            ' the 3 lines of code above can be written on a single:
            'pDR("DateTo") = Now.AddDays(1 - Weekday(Now)).Date.AddDays(pDR("CompilationDay") - 1).Add(CDate(pDR("CompilationTime")).TimeOfDay)
            If pDR("SubmissionDate") < pDR("DateTo") Then pDR("DateTo") = CDate(pDR("DateTo")).AddDays(-7)
        Next pDR

        pDT.DefaultView.Sort = "DateTo"
        For p = 0 To pDT.DefaultView.Count - 1
            pDRV = pDT.DefaultView(p)
            If p = 0 Then
                pDRV("DateFrom") = pDT.DefaultView(pDT.DefaultView.Count - 1)("DateTo")
                pDRV("DateFrom") = CDate(pDRV("DateFrom")).AddDays(-7)
            Else
                pDRV("DateFrom") = pDT.DefaultView(p - 1)("DateTo")
            End If
        Next p

        Return pDT.DefaultView

    End Function

    Public Function CurrentBranchDesignation() As Integer

        Dim pSql As String
        Dim pInt As Integer

        pSql = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 0)
        If Integer.TryParse(pSql, pInt) Then
            Return pInt
        End If

    End Function

    Public Function CurrentBranch() As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Designation", 0)
        If IsNumeric(pSql) = False Then pSql = 0
        pSql = "SELECT DavaoUnifiedPayroll.dbo.LibraryBranch.ID AS BranchID" & _
            " ,DavaoUnifiedPayroll.dbo.LibraryBranch.Name AS BranchName " & _
            ",LibraryBranches.ID" & _
            ",LibraryBranches.CustomerID" & _
            ",LibraryBranches.BranchType" & _
            ",tDC.BranchID AS DistributionCenterNumericId " & vbCrLf &
            ",tDC.ID AS DistributionCenterId " & vbCrLf &
            ",bDC.Name AS DistributionCenterName" & vbCrLf &
            " FROM DavaoUnifiedPayroll.dbo.LibraryBranch " & _
            " JOIN LibraryBranches ON LibraryBranches.BranchID = DavaoUnifiedPayroll.dbo.LibraryBranch.ID" & _
            " LEFT JOIN LibraryBranches tDC ON tDC.ID = LibraryBranches.DistributionCenterID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch bDC ON bDC.ID = tDC.BranchID" & vbCrLf &
            " WHERE DavaoUnifiedPayroll.dbo.LibraryBranch.ID = " & CInt(pSql)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    <Obsolete("Use PreviousSchedule instead")> _
    Public Function PreviousSchedule_old(ByVal vID As String) As DataView

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pDRV As DataRowView
        Dim p As Integer
        Dim pNow As Date

        pDT = Schedules(vID)
        pNow = Now
        pDT.Columns.Add("DateFrom", GetType(Date))
        pDT.Columns.Add("DateTo", GetType(Date))
        pDT.Columns.Add("SubmissionDate", GetType(Date))

        For Each pDR In pDT.Rows
            pDR("SubmissionDate") = pNow.AddDays(1 - Weekday(pNow)).Date  ' the Sunday of the current week
            pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(pDR("SubmissionDay") - 1) ' add the compilation day
            pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).Add(CDate(pDR("SubmissionTime")).TimeOfDay)   ' add the compilation time
            If pDR("SubmissionDate") > pNow Then pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(-7)

            pDR("DateTo") = pNow.AddDays(1 - Weekday(pNow)).Date  ' the Sunday of the current week
            pDR("DateTo") = CDate(pDR("DateTo")).AddDays(pDR("CompilationDay") - 1) ' add the compilation day
            pDR("DateTo") = CDate(pDR("DateTo")).Add(CDate(pDR("CompilationTime")).TimeOfDay)   ' add the compilation time
            ' the 3 lines of code above can be written on a single:
            'pDR("DateTo") = Now.AddDays(1 - Weekday(Now)).Date.AddDays(pDR("CompilationDay") - 1).Add(CDate(pDR("CompilationTime")).TimeOfDay)
            If pDR("DateTo") > pNow Then pDR("DateTo") = CDate(pDR("DateTo")).AddDays(-7)
        Next pDR

        pDT.DefaultView.Sort = "DateTo"
        For p = 0 To pDT.DefaultView.Count - 1
            pDRV = pDT.DefaultView(p)
            If p = 0 Then
                pDRV("DateFrom") = pDT.DefaultView(pDT.DefaultView.Count - 1)("DateTo")
                pDRV("DateFrom") = CDate(pDRV("DateFrom")).AddDays(-7)
            Else
                pDRV("DateFrom") = pDT.DefaultView(p - 1)("DateTo")
            End If
        Next p
        pDT.DefaultView.Sort = "SubmissionDate DESC"

        Return pDT.DefaultView

    End Function

    Private Function PreviousSchedule(ByVal vID As String) As DataView

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pNow As Date

        pDT = Schedules(vID)
        pNow = Now
        pDT.Columns.Add("DateTo", GetType(Date))
        pDT.Columns.Add("SubmissionDate", GetType(Date))

        For Each pDR In pDT.Rows
            pDR("SubmissionDate") = pNow.AddDays(1 - Weekday(pNow)).Date  ' the Sunday of the current week
            pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(pDR("SubmissionDay") - 1) ' add the compilation day
            pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).Add(CDate(pDR("SubmissionTime")).TimeOfDay)   ' add the compilation time
            If pDR("SubmissionDate") > pNow Then pDR("SubmissionDate") = CDate(pDR("SubmissionDate")).AddDays(-7)

            pDR("DateTo") = pNow.AddDays(1 - Weekday(pNow)).Date  ' the Sunday of the current week
            pDR("DateTo") = CDate(pDR("DateTo")).AddDays(pDR("CompilationDay") - 1) ' add the compilation day
            pDR("DateTo") = CDate(pDR("DateTo")).Add(CDate(pDR("CompilationTime")).TimeOfDay)   ' add the compilation time
            ' the 3 lines of code above can be written on a single:
            'pDR("DateTo") = Now.AddDays(1 - Weekday(Now)).Date.AddDays(pDR("CompilationDay") - 1).Add(CDate(pDR("CompilationTime")).TimeOfDay)
            If pDR("DateTo") > pDR("SubmissionDate") Then pDR("DateTo") = CDate(pDR("DateTo")).AddDays(-7)
        Next pDR

        pDT.DefaultView.Sort = "SubmissionDate DESC"

        Return pDT.DefaultView

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vCurrent">True = current, False = previous</param>
    ''' <param name="vDcBranchID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSchedules(ByVal vCurrent As Boolean, Optional ByVal vDcBranchID As String = Nothing) As DataTable

        Dim pDt As DataTable
        Dim pDr As DataRow
        Dim pDvSched As DataView
        Dim pDtBranches As DataTable
        Dim pDrBranch As DataRow

        pDt = New DataTable
        With pDt.Columns
            .Add("ID")
            .Add("BranchID", GetType(Integer))
            .Add("Name")
            .Add("DateTo", GetType(Date))
            .Add("SubmissionDate", GetType(Date))
        End With
        pDtBranches = GetAll(vDcBranchID)
        For Each pDrBranch In pDtBranches.Rows
            If vCurrent Then
                pDvSched = CurrentSchedule(CStr2(pDrBranch("ID")))
            Else
                pDvSched = PreviousSchedule(CStr2(pDrBranch("ID")))
            End If
            If pDvSched.Count > 0 Then
                pDvSched.Sort = "SubmissionDate"
                pDr = pDt.NewRow
                pDr("ID") = CStr2(pDrBranch("ID"))
                pDr("BranchID") = pDrBranch("BranchID")
                pDr("Name") = pDrBranch("Name")
                pDr("DateTo") = pDvSched(0)("DateTo")
                pDr("SubmissionDate") = pDvSched(0)("SubmissionDate")
                pDt.Rows.Add(pDr)
            End If
        Next pDrBranch

        Return pDt

    End Function

    Public Function GetModifiedQr(ByVal vID As String) As DataView

        Dim pDV As DataView
        Dim pLastServed As Date?

        pLastServed = LastServedDate(vID)
        pDV = PreviousSchedule(vID)

        '   do not generate QR order if the next submission date is not later than the "last served" date
        If pDV.Count > 0 Then
            If pLastServed Is Nothing _
               OrElse pLastServed < pDV(0)("SubmissionDate") Then

                ' ok
                Return pDV
            Else
                ' not ok
                Return Nothing
            End If
        End If

    End Function

#If ProjName = "ERP" Then
    Public Function CreateAndTagQr(ByVal vID As String, ByVal vDateTo As Date, ByVal vSubmissionDate As Date, ByVal vSalesOrderID As String) As Boolean

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String

        pSql = "SELECT TOP 1 SubmissionDate" & _
            " FROM TransQrModified" & _
            " WHERE BranchID = " & ReadyString(vID) & _
            " AND IsCancelled = 0" & _
            " AND DateTo = " & ReadyString(vDateTo) & _
            " AND SubmissionDate = " & ReadyString(vSubmissionDate)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then Return True

        pIU = New InsertUpdate("TransQrModified")
        pIU("ID") = "newid()"
        pIU("BranchID") = ReadyString(vID)
        pIU("Num") = mProData.MaxNum("TransQrModified", vID) + 1
        pIU("UserID") = modClient.EmployeeWebID
        pIU("LastUpdated") = "GETDATE()"
        pIU("DateFrom") = ReadyString(vDateTo)
        pIU("DateTo") = ReadyString(vDateTo)
        pIU("SubmissionDate") = ReadyString(vSubmissionDate)
        pIU("QRSalesOrderID") = ReadyString(vSalesOrderID)
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        Return True

    End Function
#End If

    Public Function LastServedDate(ByVal vID As String) As Date?

        Dim pDT As DataTable
        Dim pSql As String
        Dim pLastServed As Date

        pSql = "SELECT TOP 1 SubmissionDate" & _
            " FROM TransQrModified" & _
            " WHERE BranchID = " & ReadyString(vID) & _
            " AND IsCancelled = 0" & _
            " AND QRSalesOrderID IS NOT NULL" & _
            " ORDER BY SubmissionDate DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)("SubmissionDate")
            'Else
            '    pLastServed = Date.MinValue
        End If

        'Return pLastServed

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vDcBranchID">This must be an integer</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAll(Optional ByVal vDcBranchID As String = Nothing) As DataTable

        If CStr2(vDcBranchID).Length > 0 Then
            Return GetByWhere("bDC.ID = " & vDcBranchID)
        Else
            Return GetByWhere()
        End If

    End Function

    Public Function GetWithNames() As DataTable

        Return GetByWhere("b.Name IS NOT NULL")

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vID">LibraryBranches.ID of the CPO</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByCpo(Optional ByVal vID As String = Nothing) As DataTable

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pDV As DataView

        pDT = GetAll()
        pDT.Columns.Add("DcBranchID")
        pDT.Columns.Add("CpoBranchID")
        For Each pDR In pDT.Rows
            Select Case CStr2(pDR("BranchType"))
                Case 0  ' ordinary branch
                    pDV = New DataView(pDT, "ID = " & ReadyString(pDR("ParentBranchID")) & _
                        " AND BranchType >= 1", "", DataViewRowState.CurrentRows)
                    If pDV.Count > 0 Then
                        pDR("DcBranchID") = CStr2(pDV(0)("ID"))
                        If pDV(0)("BranchType") = 2 Then
                            pDR("CpoBranchID") = CStr2(pDV(0)("ID"))
                        Else
                            pDV = New DataView(pDT, "ID = " & ReadyString(pDV(0)("ParentBranchID")) & _
                                " AND BranchType = 2", "", DataViewRowState.CurrentRows)
                            If pDV.Count > 0 Then
                                pDR("CpoBranchID") = CStr2(pDV(0)("ID"))
                            Else
                                Stop2()
                            End If
                        End If
                    Else
                        'Stop2()
                    End If

                Case 1  ' dc
                    pDR("DcBranchID") = CStr2(pDR("ID"))
                    If pDR("BranchType") = 2 Then
                        pDR("CpoBranchID") = CStr2(pDR("ID"))
                    Else
                        pDV = New DataView(pDT, "ID = " & ReadyString(pDR("ParentBranchID")) & _
                      " AND BranchType = 2", "", DataViewRowState.CurrentRows)
                        If pDV.Count > 0 Then
                            pDR("CpoBranchID") = CStr2(pDV(0)("ID"))
                        Else
                            'Stop2()
                        End If
                    End If

                Case 2  ' cpo
                    pDR("DcBranchID") = CStr2(pDR("ID"))
                    pDR("CpoBranchID") = CStr2(pDR("ID"))

            End Select
        Next pDR

        If CStr2(vID).Length > 0 Then
            pDT = New DataView(pDT, "CpoBranchID = " & ReadyString(ReadyGuid(vID)), "", DataViewRowState.CurrentRows).ToTable
        End If

        Return pDT

    End Function

    Public Function GetNoParent() As DataTable

        Return GetByWhere("LibraryBranches.ParentBranchID IS NULL AND ISNULL(LibraryBranches.BranchType, 0) <> 2")

    End Function

    Private Function GetByWhere(Optional ByVal vWhere As String = Nothing) As DataTable

        Dim pSql As String

        pSql = "SELECT b.Name" & _
            ",LibraryBranches.ID" & _
            ",LibraryBranches.BranchID" & _
            ",LibraryBranches.DistributionCenterID" & _
            ",LibraryBranches.ParentBranchID" & _
            ",LibraryBranches.BranchType" & _
            ",LibraryBranches.AsDc" & _
            ",LibraryBranches.AsBranch" & _
            ",LibraryCustomers.ID AS CustomerID" & _
            ",LibraryCustomers.CompanyName" & _
            ",b.ID AS Num" &
            ",LibraryBranches.MachineNumsToQr"
        pSql &= " FROM LibraryBranches" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch b ON b.ID = LibraryBranches.BranchID " & _
            " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = LibraryBranches.CustomerID" & _
            " LEFT JOIN LibraryBranches tDC ON tDC.ID = LibraryBranches.DistributionCenterID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch bDC ON bDC.ID = tDC.BranchID" & _
            " LEFT JOIN LibraryBranches tCpo ON tCpo.ID = LibraryBranches.CpoId" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch bCpo ON bCpo.ID = tCpo.BranchID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmployees ON tblEmployees.ID = LibraryBranches.UserID"
        If CStr2(vWhere).Length > 0 Then
            pSql &= " WHERE " & vWhere
        End If
        pSql &= " ORDER BY b.Name"

        Return mProData.Execute(pSql)

    End Function

    ''' <summary>
    ''' </summary>
    ''' <param name="vDcBranchID">This must be an integer</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetWithoutSchedule(Optional ByVal vDcBranchID As String = Nothing) As DataTable

        Dim pSql As String

        pSql = "SELECT DISTINCT b.Name" & _
            ",LibraryBranches.ID" & _
            ",LibraryBranches.BranchID" & _
            ",LibraryBranches.DistributionCenterID" & _
            ",LibraryCustomers.ID AS CustomerID"
        pSql &= " FROM DavaoUnifiedPayroll.dbo.LibraryBranch b " & _
            " LEFT JOIN LibraryBranches ON b.ID = LibraryBranches.BranchID " & _
            " LEFT JOIN LibraryCustomers ON LibraryCustomers.ID = LibraryBranches.CustomerID" & _
            " LEFT JOIN LibraryBranches tDC ON tDC.ID = LibraryBranches.DistributionCenterID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch bDC ON bDC.ID = tDC.BranchID" & _
            " LEFT JOIN LibraryBranches tCpo ON tCpo.ID = LibraryBranches.CpoId" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch bCpo ON bCpo.ID = tCpo.BranchID" & _
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmployees ON tblEmployees.ID = LibraryBranches.UserID" & _
            " LEFT JOIN LibraryBranchSchedules ON LibraryBranchSchedules.BranchID = LibraryBranches.ID" & _
            " WHERE LibraryBranchSchedules.ID IS NULL"
        If CStr2(vDcBranchID).Length > 0 Then
            pSql &= " AND bDC.ID = " & vDcBranchID
        End If

        Return mProData.Execute(pSql)

    End Function

    Public Function GetCustomerID(ByVal vBranchID As Integer) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT CustomerID" & _
            " FROM LibraryBranches" & _
            " WHERE BranchID = " & vBranchID
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0)("CustomerID"))
        Else
            Return ""
        End If

    End Function

    ''' <summary>
    ''' Copy branches from DavaoUnifiedPayroll.LibraryBranch to Wholesale.LibraryBranches (if not already exisiting)
    ''' </summary>
    ''' <returns>number of rows copied</returns>
    Public Function CopyBranches(Optional ByVal vCustomerID As String = "", Optional ByVal vDistributionCenterID As String = "",
                                 Optional ByVal vCpoId As String = "", Optional ByVal vMachineNumsToQr As String = "",
                                 Optional ByVal vStoreSize As Double = 0, Optional ByVal vLaborBudget As Double = 0,
                                 Optional ByVal vIsActive As Boolean = False, Optional ByVal BranchID As String = vbNullString,
                                 Optional ByVal StoreOwnership As String = vbNullString, Optional ByVal ContractStart As String = vbNullString,
                                 Optional ByVal ContractEnd As String = vbNullString, Optional ByVal TelephoneNumbers As String = vbNullString,
                                 Optional ByVal Street As String = vbNullString, Optional ByVal DistrictID As String = vbNullString,
                                 Optional ByVal CityID As String = vbNullString, Optional ByVal ProvinceID As String = vbNullString, Optional ByVal CountryID As String = vbNullString,
                                 Optional ByVal EmailAddress As String = vbNullString, Optional ByVal SalesTargetPerDay As Decimal = 0,
                                 Optional ByVal SalesPerSquareMeter As Decimal = 0) As Integer

        Dim pDT As DataTable
        Dim pSql As String
        Dim pIU As InsertUpdate
        Dim pMissingCount As Integer

        Dim pWholesalePrice As String
        Dim pWholesaleNumericID As Integer
        Dim pRetailPrice As String
        Dim pRetailNumericID As Integer
        Dim pBranchCost As String
        Dim pBranchCostNumericID As Integer

#If Debug = 0 Then
        Try
#End If
        pSql = "SELECT COUNT(*)" & _
            " FROM DavaoUnifiedPayroll.dbo.LibraryBranch b" & _
            " LEFT JOIN LibraryBranches ON LibraryBranches.BranchID = b.ID" & _
            " WHERE LibraryBranches.ID IS NULL"
        pMissingCount = CDec2(mProData.ExecuteScalar(pSql))
        If pMissingCount = 0 Then Return 0

        pWholesalePrice = mParent.SystemDefaults.GetOrAddSystemDefault("Wholesale Price ID", "{01000012-0004-0000-0000-000000000000}")
        If IsGuid(pWholesalePrice) = False Then pWholesalePrice = "{01000012-0004-0000-0000-000000000000}"
        pSql = "SELECT PriceLevelNumericID" & _
            " FROM LibraryPriceLevels" & _
            " WHERE ID = " & ReadyString(pWholesalePrice)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pWholesaleNumericID = pDT.Rows(0)("PriceLevelNumericID")
        End If

        pRetailPrice = mParent.SystemDefaults.GetOrAddSystemDefault("Retail Price ID", "{01000012-0005-0000-0000-000000000000}")
        If IsGuid(pRetailPrice) = False Then pRetailPrice = "{01000012-0005-0000-0000-000000000000}"
        pSql = "SELECT PriceLevelNumericID" & _
            " FROM LibraryPriceLevels" & _
            " WHERE ID = " & ReadyString(pRetailPrice)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pRetailNumericID = pDT.Rows(0)("PriceLevelNumericID")
        End If

        pBranchCost = mParent.SystemDefaults.GetOrAddSystemDefault("Branch Cost ID", "{01000012-0006-0000-0000-000000000000}")
        If IsGuid(pBranchCost) = False Then pBranchCost = "{01000012-0006-0000-0000-000000000000}"
        pSql = "SELECT PriceLevelNumericID" & _
            " FROM LibraryPriceLevels" & _
            " WHERE ID = " & ReadyString(pBranchCost)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pBranchCostNumericID = pDT.Rows(0)("PriceLevelNumericID")
        End If

        pIU = New InsertUpdate
        pIU.TableName = "LibraryBranches"
        If BranchID <> vbNullString Then
            pIU("ID") = ReadyString(BranchID)
        End If
        pIU("BranchID") = "b.ID"
        pIU("WholesalePrice") = ReadyString(pWholesalePrice)
        pIU("RetailPrice") = ReadyString(pRetailPrice)
        pIU("BranchCost") = ReadyString(pBranchCost)
        pIU("StoreSize") = 0
        pIU("WholesaleNumericID") = pWholesaleNumericID
        pIU("RetailNumericID") = pRetailNumericID
        pIU("BranchCostNumericID") = pBranchCostNumericID
        pIU("CustomerID") = If(vCustomerID = "", "NULL", ReadyString(vCustomerID))
        pIU("DistributionCenterID") = If(vDistributionCenterID = "", "NULL", ReadyString(vDistributionCenterID))
        pIU("CpoId") = If(vCpoId = "", "NULL", ReadyString(vCpoId))
        pIU("MachineNumsToQr") = If(vMachineNumsToQr = "", "NULL", ReadyString(vMachineNumsToQr))
        pIU("StoreSize") = If(vStoreSize <= 0, 0, ReadyString(vStoreSize))
        pIU("LaborBudget") = If(vLaborBudget <= 0, "NULL", ReadyString(vLaborBudget))
        pIU("IsActive") = ReadyString(vIsActive)
        'Added by: Ryan Pillerin
        pIU("StoreOwnership") = ReadyString(StoreOwnership)
        pIU("ContractStart") = If(ContractStart = vbNullString, ReadyString(Now), ReadyString(CDate(ContractStart)))
        pIU("ContractEnd") = If(ContractEnd = vbNullString, ReadyString(Now), ReadyString(CDate(ContractEnd)))
        pIU("TelephoneNumbers") = If(TelephoneNumbers = vbNullString, "NULL", ReadyString(TelephoneNumbers))
        pIU("Street") = If(Street = vbNullString, "NULL", ReadyString(Street))
        pIU("DistrictID") = If(DistrictID = vbNullString, "NULL", ReadyString(DistrictID))
        pIU("CityID") = If(CityID = vbNullString, "NULL", ReadyString(CityID))
        pIU("ProvinceID") = If(ProvinceID = vbNullString, "NULL", ReadyString(ProvinceID))
        pIU("CountryID") = If(CountryID = vbNullString, "NULL", ReadyString(CountryID))
        pIU("EmailAddress") = If(EmailAddress = vbNullString, "NULL", ReadyString(EmailAddress))
        pIU("SalesTargetPerDay") = SalesTargetPerDay
        pIU("SalesPerSquareMeter") = SalesPerSquareMeter
        pIU("UserID") = "b.RegisteredBy"
        pIU("LastUpdated") = "GETDATE()"
        pIU.FromSql = " FROM DavaoUnifiedPayroll.dbo.LibraryBranch b" & _
            " LEFT JOIN LibraryBranches ON LibraryBranches.BranchID = b.ID" & _
            " WHERE LibraryBranches.ID IS NULL"
        pSql = pIU.GetInsert(True)
        pSql = mProData.ExecuteSQL(pSql)

#If Debug = 0 Then
        Catch ex As Exception
#If _MYTYPE <> "Web" Then
                        ' just ignore any error on "Release" version
            ShowMessage(ex.Message, "Error", , MessageBoxIcon.Exclamation)
#End If
        End Try
#End If

        Return pSql

    End Function

    Public Function LastImported(ByVal vNum As Integer) As Nullable(Of Date)

        Dim pDT As DataTable
        Dim pSql, pSql2 As String

#If Debug = 0 Then
        Try
#End If
        'BEGIN the following sql does not work
        'pSql = "SELECT TOP 1 LastUpdated" & _
        '    " FROM POSSales" & _
        '    " WHERE BranchID = " & vNum & _
        '    " ORDER BY LastUpdated DESC"
        'pSql &= " UNION "
        'pSql &= "SELECT TOP 1 LastUpdated" & _
        '    " FROM POSCashSlipSales" & _
        '    " WHERE BranchID = " & vNum & _
        '    " ORDER BY LastUpdated DESC"
        'pSql = "SELECT TOP 1 LastUpdated" & _
        '    " FROM (" & pSql & ") t" & _
        '    " ORDER BY LastUpdated DESC"
        'END (dont delete for reference)

        ' the following sql works
        'http://sqlblogcasts.com/blogs/tonyrogerson/archive/2007/05/18/using-top-and-order-by-with-union-union-all.aspx
        pSql2 = "SELECT TOP 1 LastUpdated" & _
            " FROM POSSales" & _
            " WHERE BranchID = " & vNum & _
            " ORDER BY LastUpdated DESC"
        pSql = "SELECT * FROM (" & pSql2 & ") AS d"
        pSql &= " UNION "
        pSql2 = "SELECT TOP 1 LastUpdated" & _
            " FROM POSCashSlipSales" & _
            " WHERE BranchID = " & vNum & _
            " ORDER BY LastUpdated DESC"
        pSql &= "SELECT * FROM (" & pSql2 & ") AS d"
        pSql = "SELECT TOP 1 LastUpdated" & _
            " FROM (" & pSql & ") t" & _
            " ORDER BY LastUpdated DESC"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)(0)
        End If
#If Debug = 0 Then
        Catch ex As Exception
#If _MYTYPE <> "Web" Then
            ' just ignore any error on "Release" version
            ShowMessage(ex.Message, "Error", , MessageBoxIcon.Exclamation)
#End If
        End Try
#End If

    End Function

    Public Function GetCpo(ByVal vID As String) As String

        Dim pSql As String

        pSql = "SELECT CpoID" &
            " FROM LibraryBranches" &
            " WHERE ID = " & ReadyString(vID)
        Return CStr2(mProData.ExecuteScalar(pSql))

    End Function

    Public Function LastQrSalesOrder(vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 TransSalesOrder.OrderNo" & vbCrLf & _
            " ,TransSalesOrder.LastUpdated" & vbCrLf & _
            " FROM TransSalesOrder" & vbCrLf & _
            " JOIN TransQrModified ON TransQrModified.QRSalesOrderID = TransSalesOrder.ID" & vbCrLf & _
            " WHERE TransQrModified.BranchID = " & ReadyString(vID) &
            " ORDER BY TransSalesOrder.LastUpdated" & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vCustomerAddressID">LibraryCustomersAddresses.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByCustomerAddressID(vCustomerAddressID As String) As DataRow

        Dim pSql As String

        pSql = "SELECT ID" &
            " FROM DavaoUnifiedPayroll.dbo.LibraryBranch tblBranch" &
            " WHERE CustomerAddressID = " & ReadyString(vCustomerAddressID)

        Return mProData.ExecuteRow(pSql)

    End Function
End Class
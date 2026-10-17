Public Class LibraryCustomers

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function Addresses(ByVal vID As String) As DataTable

        Dim pSql As String

        pSql = "SELECT LibraryCustomerAddresses.ID" & _
            ",CityID" & _
            ",LibraryCities.Name AS CityName" & _
            ",Street" & _
            ",PhoneNum" & _
            " FROM LibraryCustomerAddresses" & _
            " JOIN LibraryCities" & _
            " ON LibraryCustomerAddresses.CityID = LibraryCities.ID" & _
            " WHERE CONVERT(varchar(36), CustomerID) = " & ReadyString(vID)
        Return mProData.Execute(pSql)

    End Function

    Public Function Persons(ByVal vID As String) As DataTable

        Dim pSql As String

        pSql = "SELECT LibraryCustomerPersons.ID" & _
            ",LibraryCustomerPersons.PersonID" & _
            ",LastName + ', ' + FirstName + ' ' + MiddleName AS FullName" & _
            ",PhoneNum" & _
            " FROM LibraryCustomerPersons" & _
            " JOIN LibraryPersons" & _
            " ON LibraryCustomerPersons.PersonID = LibraryPersons.ID" & _
            " WHERE CONVERT(varchar(36), LibraryCustomerPersons.CustomerID) = " & ReadyString(vID)

        Return mProData.Execute(pSql)

    End Function

    Public Function LoanPayables(ByVal vID As String, ByVal vDate As Date, Optional ByRef oLog As String = "") As Decimal

        Dim pDTLoans, pDTPayments As DataTable
        Dim pDR As DataRow
        Dim pDRV As DataRowView
        Dim pAmount, pLoanAmount, pInterest, pAmountDebit As Decimal
        Dim pDate As Date

        Return 0
        oLog = ""
        pDTLoans = mProData.Execute("SELECT Amount" & _
            ",InterestRate" & _
            ",Date1" & _
            " FROM TransLoans" & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY Date1")
        pDTPayments = mProData.Execute("SELECT Amount" & _
            ",CONVERT(varchar, Date1, 111) AS Date1" & _
            " FROM TransLoanPayments" & _
            " WHERE CustomerID = " & ReadyString(vID))
        pDTPayments.Columns.Add("AmountDebited", GetType(Decimal))

        For Each pDR In pDTLoans.Rows
            pDate = CDate(pDR("Date1")).Date
            pLoanAmount = Format(CDec(pDR("Amount") * (1 + pDR("InterestRate") / 100)), "0.00")
            pAmount += pLoanAmount
            oLog &= Format(pDate, cGlobal.FormatDate) & _
                ". Burrows Php " & Format(pLoanAmount, cGlobal.FormatCurrency) & _
                " @ " & Format(CDec(pDR("InterestRate")), "0.00") & "% interest." & vbCrLf
            oLog &= "Running Balance: Php " & Format(pAmount, cGlobal.FormatCurrency) & vbCrLf & vbCrLf
            Do
                pDTPayments.DefaultView.RowFilter = "Date1 >= '" & Format(CDate(pDate), "yyyy/MM/dd") & "'" & _
                    " AND Date1 < '" & Format(DateAdd(DateInterval.Month, 1, pDate), "yyyy/MM/dd") & "'" & _
                    " AND ISNULL(AmountDebited, 0) < Amount"
                For Each pDRV In pDTPayments.DefaultView
                    pAmountDebit = CDec2(pDRV("AmountDebited"))
                    If pDRV("Amount") <= pDRV("Amount") - pAmountDebit Then
                        pAmountDebit += pAmountDebit
                    End If
                    pDRV("AmountDebited") = pAmountDebit

                    pAmount -= pAmountDebit
                    pLoanAmount -= pAmountDebit

                    oLog &= Format(pDate, cGlobal.FormatDate) & _
                        ". Pays Php " & Format(CDec(pDRV("Amount")), cGlobal.FormatCurrency) & vbCrLf & _
                        "Demitted Amount: Php " & Format(pAmountDebit, cGlobal.FormatCurrency) & vbCrLf
                    oLog &= "Running Balance: Php " & Format(pAmount, cGlobal.FormatCurrency) & vbCrLf & vbCrLf
                Next pDRV

                If pLoanAmount <= 0 Then Exit Do
                If DateAdd(DateInterval.Month, 1, pDate) > vDate Then Exit Do
                pDate = DateAdd(DateInterval.Month, 1, pDate)
                pInterest = Format(CDec(pLoanAmount * (pDR("InterestRate") / 100)), "0.00")
                pLoanAmount += pInterest
                pAmount += pInterest

                oLog &= Format(pDate, cGlobal.FormatDate) & _
                    ". Interest Php " & Format(pInterest, cGlobal.FormatCurrency) & _
                    " is added" & vbCrLf
                oLog &= "Running Balance: Php " & Format(pAmount, cGlobal.FormatCurrency) & vbCrLf & vbCrLf
            Loop
        Next pDR

        Return pAmount

    End Function

    Public Function LatestLoan(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 Date1" & _
            ",Num" & _
            " FROM TransLoans" & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY Date1 DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function LatestLoanPayment(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 Date1" & _
            ",Num" & _
            " FROM TransLoanPayments" & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY Date1 DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    '---Kim Added: 06/15/2010
    Public Function GetCustomerAgent(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        'pSql = "SELECT TOP 100 PERCENT " & _
        '           "(DavaoUniFiedPayroll.dbo.EmployeeProfiles.LastName + ', ' +" & _
        '           "DavaoUniFiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' +" & _
        '           "LEFT(DavaoUniFiedPayroll.dbo.EmployeeProfiles.MiddleName,1) + '.') AS CustomerAgent " & _
        '       "FROM LibraryCustomersAgents INNER JOIN " & _
        '          "DavaoUniFiedPayroll.dbo.EmployeeProfiles ON LibraryCustomersAgents.EmployeeID = DavaoUniFiedPayroll.dbo.EmployeeProfiles.ID " & _
        '      " Where (LibraryCustomersAgents.CustomerID ='" & vID & "') " & _
        '      " ORDER BY DavaoUniFiedPayroll.dbo.EmployeeProfiles.LastName + ', ' + DavaoUniFiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' + " & strPayrollDatabase & ".dbo.EmployeeProfiles.MiddleName ASC"

        pSql = "SELECT TOP 100 PERCENT " & _
                    "(DavaoUniFiedPayroll.dbo.EmployeeProfiles.LastName + ', ' +" & _
                    "DavaoUniFiedPayroll.dbo.EmployeeProfiles.FirstName + ' ' +" & _
                "LEFT(DavaoUniFiedPayroll.dbo.EmployeeProfiles.MiddleName,1) + '.') AS CustomerAgent " & _
                "FROM LibraryCustomersAgents INNER JOIN " & _
                    "DavaoUniFiedPayroll.dbo.EmployeeProfiles ON LibraryCustomersAgents.EmployeeID = DavaoUniFiedPayroll.dbo.EmployeeProfiles.ID " & _
                "Where (LibraryCustomersAgents.CustomerID ='" & vID & "') "

        pDT = mProData.Execute(pSql)
        GetCustomerAgent = ""
        If pDT.Rows.Count > 0 Then
            For Each ItemData As DataRow In pDT.Rows
                If Not ItemData("CustomerAgent") IsNot DBNull.Value Then
                    GetCustomerAgent = GetCustomerAgent & "/" & ItemData("CustomerAgent")
                End If
            Next
        End If
        pDT.Dispose()

    End Function

    Public Function GetCustomerAddressCount(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT COUNT(CONVERT(VARCHAR(36),CustomerID)) AS CountCustAddress " &
                "FROM LibraryCustomersAddresses WHERE CustomerID = " & ReadyString(vID) & " " & _
                "GROUP BY CustomerID "

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function RequiresPO(ByVal vID As String) As Boolean

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT RequiresPO" & _
               " FROM LibraryBusinessTypes" & _
               " JOIN LibraryCustomers" & _
               " ON LibraryCustomers.BusinessTypeID = LibraryBusinessTypes.ID" & _
               " WHERE LibraryCustomers.ID = '" & vID & "'"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CDec2(pDT.Rows(0)("RequiresPO"))
            'If pDT.Rows(0)("RequiresPO") IsNot DBNull.Value Then
            '    Return True '--pDT.Rows(0)("RequiresPO")
            'Else
            '    Return False
            'End If

        Else
            Return False
        End If

    End Function

    Public Function GetCustomerAddressesID(ByVal vID As String) As DataRow


        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryCustomerAddresses.ID" & _
            ",CityID" & _
            ",LibraryCities.Name AS CityName" & _
            ",Street" & _
            ",PhoneNum" & _
            " FROM LibraryCustomerAddresses" & _
            " JOIN LibraryCities" & _
            " ON LibraryCustomerAddresses.CityID = LibraryCities.ID" & _
            " WHERE CONVERT(varchar(36), CustomerID) = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetCustomerAddress(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String


        pSql = "SELECT LibraryCustomersAddresses.ID,LibraryCustomersAddresses.CustomerID,ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibraryCustomersAddresses.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '')  AS Address " & _
            ",LibraryCustomersAddresses.Name" &
            " FROM LibraryCustomersAddresses" & _
            " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID " & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNamesA ON LibraryStreetNamesA.ID = LibraryCustomersAddresses.StreetID2" & _
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " LEFT JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" & _
            " LEFT JOIN LibraryCountries ON LibraryCountries.ID = LibraryProvinces.CountryID " & _
            "WHERE CONVERT(VARCHAR(36),LibraryCustomersAddresses.CustomerID) = '" & vID & "'"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetByCustomerAddressID(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = pSql & " SELECT MobizLibraryOrganizationFOWST.ID" & vbCrLf & _
            " ,dbo.SplitJoinComma(ISNULL(MobizLibraryOrganizationFOWST.StreetNumber, '') " & vbCrLf & _
            " 	+ ', ' + ISNULL(MobizLibraryOrganizationFOWST.Street, '')" & vbCrLf & _
            "     + ', ' + ISNULL(MobizLibraryCities.Name, '') " & vbCrLf & _
            "     + ', ' + ISNULL(MobizLibraryCountries.Name, '')) AS FullName" & vbCrLf & _
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf & _
            " LEFT JOIN MobizLibraryCities ON MobizLibraryOrganizationFOWST.FowstCityID = MobizLibraryCities.ID" & vbCrLf & _
            " LEFT JOIN MobizLibraryCountries ON MobizLibraryOrganizationFOWST.FowstCountryID = MobizLibraryCountries.ID" & vbCrLf & _
            " WHERE MobizLibraryOrganizationFOWST.OrganizationID = " & ReadyString(vID) & vbCrLf

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

#Region " duc "
    Public Function GetAvailableCreditLimit(ByVal vID As String, _
        Optional ByRef oMaxDaysOverDue As Integer = 0, _
        Optional ByRef oError As String = Nothing, _
        Optional ByRef oMinDueDate As String = Nothing) As Double

        Dim pSql As String
        Dim pDT As DataTable
        Dim pBalance As Decimal
        Dim pCreditLimit As Decimal

        On Error GoTo ErrHandler

        GetAvailableCreditLimit = 0
        oMaxDaysOverDue = 0

        '// get from cache
        'pSql = "SELECT AvailableCreditLimit" & _
        '    " FROM LibraryCustomers" & _
        '    " WHERE ID = " & ReadyString(vID)

        'pDT = mProData.Execute(pSql)
        'If pDT.Rows.Count > 0 Then
        '    If pDT.Rows(0)("AvailableCreditLimit") IsNot DBNull.Value Then
        '        Return pDT.Rows(0)("AvailableCreditLimit")
        '    End If
        'End If

        pSql = "Select CreditLimit" & _
            " From LibraryCustomers " & _
            " WHERE (ID ='" & vID & "')"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            pCreditLimit = pDT.Rows(0)("CreditLimit")
        End If

        'pSql = "SELECT SUM(Balance) AS Balance" & _
        '   ",MAX(DaysOverDue) AS DaysOverDue" & _
        '   ",MIN(DueDate) AS DueDate" & _
        '   " From vwReportAgingOfAcountsOpenAccounts " & _
        '   " WHERE CustomerID = '" & vID & "'"
        pSql = "spReportAgingOfAcountsOpenAccounts '" & vID & "'"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            oMaxDaysOverDue = CDec2(pDT.Rows(0)("DaysOverDue"))
            If IsDate(pDT.Rows(0)("DueDate")) Then
                oMinDueDate = pDT.Rows(0)("DueDate")
            End If
            pBalance = CDec2(pDT.Rows(0)("Balance"))
        End If

        GetAvailableCreditLimit = pCreditLimit - pBalance - UninvoicedOrderAmount(vID) - PdcAmount(vID)

        '// update cache
        pSql = "UPDATE LibraryCustomers" & _
            " SET AvailableCreditLimit = " & GetAvailableCreditLimit & _
            ",AccountsLastUpdated = GetDate()"
        If IsDate(oMinDueDate) Then
            pSql &= ",MinDueDate = " & ReadyString(oMinDueDate)
        Else
            pSql &= ",MinDueDate = NULL"
        End If
        pSql &= " WHERE ID = " & ReadyString(vID)

        mProData.ExecuteSQL(pSql)

        oError = ""

        Exit Function
ErrHandler:
        Debug.Print("clsLibCustomers.GetAvailableCreditLimit " & Err.Description)

    End Function

    Public Function UninvoicedOrderAmount(ByVal vID As String) As Decimal

        Dim pDT As DataTable
        Dim pSql As String

        On Error GoTo Eror

        pSql = "SELECT ISNULL(SUM(SubTotal), 0) AS SubTotal" & _
            " FROM TransSalesOrder" & _
            " JOIN TransSalesOrderItems" & _
            " ON TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID" & _
            " LEFT JOIN TransInvoice" & _
            " ON TransInvoice.SalesOrderID = TransSalesOrder.ID" & _
            " AND TransInvoice.IsCancelled = 0" & _
            " WHERE TransSalesOrder.IsCancelled = 0" & _
            " AND TransInvoice.ID IS NULL" & _
            " AND TransSalesOrder.CustomerID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        UninvoicedOrderAmount = pDT.Rows(0)(0)

        Exit Function
Eror:
        Debug.Print("clsLibCustomers.UninvoicedOrderAmount: `" & Err.Description)

    End Function

    Public Function PdcAmount(ByVal vID As String) As Decimal

        Dim pDT As DataTable
        Dim pSql As String

        On Error GoTo Eror
        pSql = "spGetCustomerPdcAmount " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        PdcAmount = CDec2(pDT.Rows(0)(0))

        Exit Function
Eror:
        Debug.Print("clsLibCustomers.PdcAmount: " & Err.Description)

    End Function

    Public Function GetAccumulatedDOPercent(ByVal vID As String) As Double

        Dim pSql As String
        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pCurrentAmount
        Dim pCurrentCount
        Dim p30Amount
        Dim p30Count
        Dim p60Amount
        Dim p60Count
        Dim p90Amount
        Dim p90Count
        Dim p120Amount
        Dim p120Count
        Dim pTotalAmount
        Dim pTotalCount

        On Error GoTo ErrHandler

        pSql = "SELECT AccumulatedDOPercent" & _
            " FROM LibraryCustomers" & _
            " WHERE ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            If pDT.Rows(0)("AccumulatedDOPercent") IsNot DBNull.Value Then
                Return pDT.Rows(0)("AccumulatedDOPercent")
            End If
        End If

        pSql = "spCustomerAging """ & vID & """"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            If pDR("DaysOverdue") < 30 Then
                pCurrentAmount = pCurrentAmount + pDR("Balance")
                pCurrentCount = pCurrentCount + 1
            ElseIf pDR("DaysOverdue") >= 30 And pDR("DaysOverdue") < 60 Then
                p30Amount = p30Amount + pDR("Balance")
                p30Count = p30Count + 1
            ElseIf pDR("DaysOverdue") >= 60 And pDR("DaysOverdue") < 90 Then
                p60Amount = p60Amount + pDR("Balance")
                p60Count = p60Count + 1
            ElseIf pDR("DaysOverdue") >= 90 And pDR("DaysOverdue") < 120 Then
                p90Amount = p90Amount + pDR("Balance")
                p90Count = p90Count + 1
            ElseIf pDR("DaysOverdue") >= 120 Then
                p120Amount = p120Amount + pDR("Balance")
                p120Count = p120Count + 1
            End If

            pTotalAmount = pTotalAmount + pDR("Balance")
            pTotalCount = pTotalCount + 1
        Next pDR

        GetAccumulatedDOPercent = ((p30Amount / IIf(pTotalAmount = 0, 1, pTotalAmount)) * 100) + _
            ((p60Amount / IIf(pTotalAmount = 0, 1, pTotalAmount)) * 100) + _
            ((p90Amount / IIf(pTotalAmount = 0, 1, pTotalAmount)) * 100) + _
            ((p120Amount / IIf(pTotalAmount = 0, 1, pTotalAmount)) * 100)

        '// update cache
        pSql = "UPDATE LibraryCustomers" & _
            " SET AccumulatedDOPercent = " & GetAccumulatedDOPercent
        pSql = pSql & ",AccountsLastUpdated = GetDate()" & _
            " WHERE ID = " & ReadyString(vID)
        mProData.Execute(pSql)

        Exit Function
ErrHandler:
        Debug.Print("clsLibCustomers.GetAccumulatedDOPercent: " & Err.Description)

    End Function

    Public Function DeleteDuplicatePriceLevels() As Integer

        Dim pDT As DataTable
        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT LibraryCustomersPriceLevel.CustomerID" & _
            " ,LibraryCustomersPriceLevel.PriceLevelID" & _
            " ,COUNT(*) AS Count" & _
            " FROM LibraryCustomersPriceLevel" & _
            " GROUP BY LibraryCustomersPriceLevel.CustomerID" & _
            " ,LibraryCustomersPriceLevel.PriceLevelID" & _
            " HAVING count(*) >= 2"
        pDT = mProData.Execute(pSql)
        For Each pDR In pDT.Rows
            pSql = "SELECT TOP " & pDR("Count") - 1 & " ID" & _
                " FROM LibraryCustomersPriceLevel" & _
                " WHERE CustomerID = " & ReadyString(pDR("CustomerID")) & _
                " AND PriceLevelID = " & ReadyString(pDR("PriceLevelID"))
            pSql = "DELETE FROM LibraryCustomersPriceLevel" & _
                " WHERE ID IN (" & pSql & ")"
            mProData.ExecuteSQL(pSql)
        Next pDR

    End Function

    Public Function GetBillingAddress(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 ID" & _
            " FROM LibraryCustomersAddresses" & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY IsBillingAddress DESC" & _
            ",IsDeliveryAddress DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0)("ID"))
        End If

    End Function

    Public Function GetDeliveryAddress(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 ID" & _
            " FROM LibraryCustomersAddresses" & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY IsDeliveryAddress DESC" & _
            ",IsBillingAddress DESC"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0)("ID"))
        End If
        pDT.Dispose()
    End Function

    Public Function GetDeliveryAddresses(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT LibraryCustomersAddresses.ID" & _
            ",LibraryDistricts.CityID" & _
            ",ISNULL(LibraryCustomersAddresses.StreetNumber, '') + ', ' + ISNULL(LibraryStreetNames.StreetName, '') + ISNULL(' cor. ' + LibraryStreetNamesA.StreetName, '') + ', ' + ISNULL(LibraryDistricts.Name + ', ', LibraryCustomersAddresses.District + ', ') + ISNULL(LibraryCities.Name + ', ', '') + ISNULL(LibraryProvinces.Name + ', ', '') + ISNULL(LibraryCountries.Name, '') AS Address"
        pSql &= " FROM LibraryCustomersAddresses" & _
            " LEFT JOIN LibraryStreetNames ON LibraryStreetNames.ID = LibraryCustomersAddresses.StreetID " & _
            " LEFT JOIN LibraryStreetNames LibraryStreetNamesA ON LibraryStreetNamesA.ID = LibraryCustomersAddresses.StreetID2" & _
            " LEFT JOIN LibraryDistricts ON LibraryDistricts.ID = LibraryStreetNames.DistrictID" & _
            " LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryDistricts.CityID" & _
            " LEFT JOIN LibraryProvinces ON LibraryProvinces.ID = LibraryCities.ProvinceID" & _
            " LEFT JOIN LibraryCountries ON LibraryCountries.ID = LibraryProvinces.CountryID" & _
            " WHERE LibraryCustomersAddresses.IsDeliveryAddress = 1" & _
            " AND LibraryCustomersAddresses.CustomerID = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' LibraryOrganizationRepresentatives
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetContactPerson(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        '  IsDefaultRepresentative field is gone

        pSql = "SELECT TOP 1 ID, (MobizLibraryOrganizationRepresentatives.LastName + ', ' + MobizLibraryOrganizationRepresentatives.FirstName + ' ' + MobizLibraryOrganizationRepresentatives.MiddleName) AS ContactPerson " & _
            " FROM MobizLibraryOrganizationRepresentatives " & _
            " WHERE OrganizationID = " & ReadyString(vID) & _
            " ORDER BY LastUpdated DESC" & _
            ",LastName, FirstName, MiddleName"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0)("ID"))
        End If
        pDT.Dispose()

    End Function

    ''' <summary>
    ''' return LibraryOrganizationEmployee.ID
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateContactPerson(vID As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String

        pID = Guid.NewGuid.ToString
        pIU = New InsertUpdate("LibraryOrganizationEmployee")
        pIU("ID") = ReadyString(pID)
        ' pIU("CustomerID") = ReadyString(vID) ' no need to specify
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    Public Function GetOrAddContactPerson(vID As String) As String

        Dim pOut As String

        pOut = GetContactPerson(vID)
        If CStr2(pOut) = "" Then
            pOut = CreateContactPerson(vID)
        End If

        Return pOut

    End Function

    Public Function GetCustomerContactPerson(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 ID, (LibraryCustomersRepresentatives.LastName + ', ' + LibraryCustomersRepresentatives.FirstName + ' ' + LibraryCustomersRepresentatives.MiddleName) AS ContactPerson " & _
            " FROM LibraryCustomersRepresentatives " & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY IsDefaultRepresentative DESC" & _
            ",LastName, FirstName, MiddleName"
        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If
        pDT.Dispose()

    End Function

    Public Function ById(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT CompanyName" & _
            " FROM MobizLibraryOrganization" & _
            " WHERE ID = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If
        pDT.Dispose()

    End Function

    Public Function AddLedger(ByVal vID As String, ByVal vTrans As String, _
        ByVal vDebit As Decimal, ByVal vCredit As Decimal, ByRef oError As String) As Boolean

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pBranchID As String ' integer

        pBranchID = mParent.SystemDefaults.GetSystemDefault("Branch Designation")
        If pBranchID Is Nothing Then
            oError = "Branch Designation has not been set"
        ElseIf Integer.TryParse(pBranchID, pBranchID) = False Then
            oError = "Branch Designation was not set properly. " & pBranchID
        End If

        pIU = New InsertUpdate("WholesaleCustomerLedger")
        pIU("BranchID") = pBranchID
        pIU("CustomerID") = ReadyString(vID)
        pIU("CustTransaction") = ReadyString(vTrans)
        pIU("Debit") = vDebit
        pIU("Credit") = vCredit
        pIU("Balance") = GetCustomerBalance(vID, vDebit, vCredit)
#If _MYTYPE = "Web" Then
        pIU("UserID") = WebSession.Instance.EmployeeID
#Else
        pIU("UserID") = User.EmployeeID
#End If
        pSql = pIU.GetInsert

        mProData.ExecuteSQL(pSql)

        oError = ""
        Return True

    End Function

    Private Function GetCustomerBalance(ByVal vCustomerID As String, ByVal vDebit As Decimal, ByVal vCredit As Decimal) As Decimal

        Dim pSql As String
        Dim pBalance As Decimal

        pSql = "SELECT Balance" & _
            " FROM WholesaleCustomerLedger " & _
            " WHERE CustomerID = " & ReadyString(vCustomerID) & _
            " ORDER BY DateTimeStamp DESC "

        pBalance = CDec2(mProData.ExecuteScalar(pSql))

        Return pBalance + vDebit - vCredit

    End Function


    Public Function IsBadAccount(ByVal vID As String) As Boolean
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT ISNULL(BadAccount, 0) AS BadAccount" & _
               " FROM LibraryCustomers" & _
               " WHERE ID = '" & vID & "'"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)("BadAccount")
        Else
            Return False
        End If
        pDT.Dispose()

    End Function

    Public Function HasCreditTerms(ByVal vID As String) As Boolean
        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT COUNT(*)" & _
               " FROM LibraryCustomersTerms" & _
               " WHERE CustomerID = '" & vID & "'"

        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        pDT.Dispose()

    End Function

    Public Function HasBackOrders(ByVal vID As String) As Boolean

        Dim strSQL As String
        Dim pDT As DataTable
        Dim strPayrollDatabase As String = "DavaoUnifiedPayroll"

        '---Added "TOP 1" to just to know if the are backorders
        strSQL = "SELECT TOP 1 TransSalesBackOrderItems.StockID," & _
            "RIGHT('0000000' + CONVERT(varchar, TransSalesOrder.OrderNo), 7) AS OrderNo," & _
            "TransSalesOrder.OrderDate," & _
            "LibraryStockInformation.StockNumber," & _
            "ISNULL(NULLIF (LTRIM(RTRIM(LibraryBrands.BrandName)), ''), '') + " & _
            "ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryProducts.ProductName)), ', '), '') + " & _
            "ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Make)), ', '), '') + " & _
            "ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.UnitSize)), ', '), '') + " & _
            "ISNULL(NULLIF (', ' + LTRIM(RTRIM(LibraryStockInformation.Content)), ', '), '') AS StockDescription," & _
            "TransSalesBackOrderItems.Quantity," & _
            "TransSalesBackOrderItems.UnitMeasure," & _
            "CONVERT(varchar, TransSalesBackOrderItems.TermDiscount) + '% ' + CONVERT(varchar, TransSalesBackOrderItems.DiscountIfPaidWithIn) + ' NET ' + TransSalesBackOrderItems.NetDue AS Terms," & _
            "TransSalesBackOrderItems.CancellationPeriod," & _
            "TransSalesBackOrderItems.ActualQuantity," & _
            "TransSalesBackOrderItems.TermDiscount," & _
            "TransSalesBackOrderItems.DiscountIfPaidWithIn," & _
            "TransSalesBackOrderItems.NetDue," & _
            "(TransSalesBackOrderItems.CancellationPeriod - CONVERT(numeric, CONVERT(datetime,(   CONVERT(varchar, Datepart(mm,GETDATE()) ) + '/' + CONVERT(varchar, Datepart(dd,GETDATE()) ) + '/' + CONVERT(varchar, Datepart(yyyy,GETDATE()) )   ) ) - (TransSalesOrder.OrderDate))) AS CancelledAfter," & _
            "TransSalesBackOrderItems.IsReadyToServe AS OnProcess," & _
            strPayrollDatabase & ".dbo.EmployeeProfiles.LastName + ', ' + " & strPayrollDatabase & ".dbo.EmployeeProfiles.FirstName + ' ' + " & strPayrollDatabase & ".dbo.EmployeeProfiles.MiddleName AS UpdatedBy," & _
            "TransSalesBackOrderItems.LastUpdated," & _
            "TransSalesBackOrderItems.CancellationPeriod AS CancellationPeriodOrig," & _
            "TransSalesBackOrderItems.ID AS SalesBackOrderID "
        strSQL = strSQL & _
            " FROM TransSalesBackOrderItems INNER JOIN " & _
            " TransSalesOrder ON TransSalesBackOrderItems.SalesOrderID = TransSalesOrder.ID INNER JOIN " & _
            " LibraryStockInformation ON TransSalesBackOrderItems.StockID = LibraryStockInformation.ID INNER JOIN " & _
            " LibraryBrands ON LibraryStockInformation.BrandID = LibraryBrands.ID INNER JOIN " & _
            " LibraryProducts ON LibraryStockInformation.ProductID = LibraryProducts.ID LEFT OUTER JOIN " & _
            " TransStockPickingList INNER JOIN " & _
            " TransStockPickingListItems ON TransStockPickingList.ID = TransStockPickingListItems.StockPickingListID ON " & _
            " TransSalesBackOrderItems.SalesOrderID = TransStockPickingListItems.SalesOrderID " & _
            " AND TransSalesBackOrderItems.SalesBackOrderID = TransStockPickingListItems.SalesBackOrderID INNER JOIN " & _
            strPayrollDatabase & ".dbo.EmployeeProfiles ON TransSalesBackOrderItems.UserID = " & strPayrollDatabase & ".dbo.EmployeeProfiles.ID " & _
            " Where (TransStockPickingList.PickingListNo Is Null) " & _
            " AND (TransSalesOrder.CustomerID = '" & vID & "') " & _
            " AND (TransSalesBackOrderItems.IsCancelled = 0) " & _
            " AND (TransSalesBackOrderItems.IsServe = 0) " & _
            " AND ((TransSalesBackOrderItems.CancellationPeriod - CONVERT(numeric, CONVERT(datetime,(   CONVERT(varchar, Datepart(mm,GETDATE()) ) + '/' + CONVERT(varchar, Datepart(dd,GETDATE()) ) + '/' + CONVERT(varchar, Datepart(yyyy,GETDATE()) )   ) ) - (TransSalesOrder.OrderDate)))>=0 ) " & _
            " ORDER BY TransSalesOrder.OrderDate"

        pDT = mProData.Execute(strSQL)
        If pDT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        pDT.Dispose()

    End Function

    Public Function HasQuotations(ByVal vID As String) As Boolean
        Dim strSQL As String
        Dim pDT As DataTable

        '---Added "TOP 1" to just to know if the are Quotations
        strSQL = "SELECT TOP 1 QuotationNo " & _
            " From TransSalesQuotations " & _
            " WHERE (CustomerID ='" & vID & "') " & _
            " AND (IsUsed=0) " & _
            " AND (IsCancelled=0)"

        pDT = mProData.Execute(strSQL)
        If pDT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        pDT.Dispose()

    End Function

    ''' <summary>
    ''' wholesale database
    ''' </summary>
    ''' <param name="vCustomerID"></param>
    ''' <param name="oGPM"></param>
    ''' <param name="oTotalGPM"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMinimumProfit(ByVal vCustomerID As String, ByRef oGPM As Double, ByRef oTotalGPM As Double) As Boolean

        Dim pRS As DataTable
        Dim pSql As String

        pSql = "SELECT MinimumLineGPM" & _
            ",MinimumTotalGPM" & _
            " FROM LibraryBusinessTypes" & _
            " JOIN LibraryCustomers" & _
            " ON LibraryCustomers.BusinessTypeID = LibraryBusinessTypes.ID" & _
            " WHERE LibraryCustomers.ID = " & ReadyString(vCustomerID)

        pRS = mProData.Execute(pSql)
        If pRS.Rows.Count > 0 Then
            If pRS.Rows(0)("MinimumLineGPM") IsNot DBNull.Value _
                And pRS.Rows(0)("MinimumTotalGPM") IsNot DBNull.Value Then

                oGPM = pRS.Rows(0)("MinimumLineGPM")
                oTotalGPM = pRS.Rows(0)("MinimumTotalGPM")
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

#End Region

    ''' <summary>
    ''' wholesale database
    ''' </summary>
    ''' <param name="vCustomerID"></param>
    ''' <param name="vDays"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TotalAmountPurchaseLastDays(ByVal vCustomerID As String, ByVal vDays As Long) As Decimal

        Dim pRS As DataTable
        Dim pSql As String

        pSql = "SELECT ISNULL(SUM(TransInvoiceItems.SubTotal),0) AS TotalPurchaseLastYear " & _
            " FROM TransInvoiceItems" & _
            " JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID " & _
            " WHERE CONVERT(varchar, TransInvoice.InvoiceDate, 111) >= CONVERT(varchar, DATEADD(day, -1 * " & vDays & ", GETDATE()), 111)" & _
            " AND TransInvoice.CustomerID = " & ReadyString(vCustomerID)

        pRS = mProData.Execute(pSql)
        If pRS.Rows.Count > 0 Then
            TotalAmountPurchaseLastDays = pRS.Rows(0)("TotalPurchaseLastYear")
        Else
            TotalAmountPurchaseLastDays = 0
        End If

        pRS.Dispose()

    End Function

    ''' <summary>
    ''' wholesale database
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MinimumTotalGPM(ByVal vID As String) As Single

        Dim pRS As DataTable
        Dim pSql As String

        pSql = "SELECT MinimumTotalGPM" & _
            " FROM LibraryCustomers" & _
            " JOIN LibraryBusinessTypes" & _
            " ON LibraryBusinessTypes.ID = LibraryCustomers.BusinessTypeID" & _
            " WHERE LibraryCustomers.ID = " & ReadyString(vID)

        pRS = mProData.Execute(pSql)
        If pRS.Rows.Count > 0 Then
            If pRS.Rows(0)("MinimumTotalGPM") IsNot DBNull.Value Then
                MinimumTotalGPM = pRS.Rows(0)("MinimumTotalGPM")
                pRS.Dispose()
                Exit Function
            End If
        End If

        Return mParent.SystemDefaults.GetOrAddSystemDefault("Minimum Total GPM", 20)

    End Function

    ''' <summary>
    ''' wholesale database
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MinimumLineGPM(ByVal vID As String) As Single

        Dim pRS As DataTable
        Dim pSql As String

        pSql = "SELECT MinimumLineGPM" & _
            " FROM LibraryCustomers" & _
            " JOIN LibraryBusinessTypes" & _
            " ON LibraryBusinessTypes.ID = LibraryCustomers.BusinessTypeID" & _
            " WHERE LibraryCustomers.ID = " & ReadyString(vID)

        pRS = mProData.Execute(pSql)
        If pRS.Rows.Count > 0 Then
            If pRS.Rows(0)("MinimumLineGPM") IsNot DBNull.Value Then
                MinimumLineGPM = pRS.Rows(0)("MinimumLineGPM")
                pRS.Dispose()
                Exit Function
            End If
        End If

        Return mParent.SystemDefaults.GetOrAddSystemDefault("Minimum Line GPM", 8)

    End Function

    Public Function CustomerNonVATorVATExempt(ByVal vCustomerID As String) As Boolean

        Dim pSQL As String
        Dim pDT As DataTable

        CustomerNonVATorVATExempt = False

        pSQL = "SELECT VATType " & _
               " FROM LibraryCustomers " & _
               " WHERE ID = '" & vCustomerID & "' "

        pDT = mProData.Execute(pSQL)
        If pDT.Rows.Count > 0 Then
            If pDT.Rows(0)("VATType") = "Exempt Sales/Receipts" Or _
               pDT.Rows(0)("VATType") = "Non Vatable" Then
                CustomerNonVATorVATExempt = True
            Else
                CustomerNonVATorVATExempt = False
            End If
        Else
            CustomerNonVATorVATExempt = False
        End If

        pDT.Dispose()

    End Function

    Public Function IsCompanyBranch(ByVal vCustomerID As String) As Boolean
        Dim pSQL As String
        Dim pDT As DataTable

        pSQL = "SELECT IsCompanyBranch " & _
            " FROM LibraryCustomers " & _
            " WHERE ID = '" & vCustomerID & "'"
        pDT = mProData.Execute(pSQL)

        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)("IsCompanyBranch")
        Else
            Return False
        End If
    End Function

    Public Function GetPriceLevels(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT PriceLevelID" &
            ",PriceLevelName" &
            " FROM LibraryCustomersPriceLevel" &
            " JOIN LibraryPriceLevels ON LibraryPriceLevels.ID = LibraryCustomersPriceLevel.PriceLevelID" &
            " WHERE CustomerID = " & ReadyString(vID) &
            " ORDER BY PriceLevelName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' Gets all unpaid (or optionally, all) invoices by a customer
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInvoices(vID As String, Optional ByVal vWithBalanceOnly As Boolean = True) As DataTable

        ' spGetCustomerOpenBalances params:
        ' @CustomerID
        ' @WithBalanceOnly
        Return mProData.Execute("spGetCustomerOpenBalances " & ReadyString(vID) & ", " & If(vWithBalanceOnly, 1, 0))

    End Function

    Public Function GetForm2307History(vID As String) As DataTable

        Dim pSql As String

        pSql = "SELECT TransBirForms.InvoiceID" & vbCrLf & _
            " ,MAX(TransBirForms.DateCreated) AS BirFormDateCreatedMax" & vbCrLf & _
            " ,COUNT(*) AS BirFormCount" & vbCrLf & _
            " ,MIN(TransBirForms.DateCreated) AS BirFormDateCreatedMin" & vbCrLf & _
            ",(SELECT TOP 1 tbl2.UserID" &
            "   FROM TransBirForms tbl2" &
            "   WHERE tbl2.InvoiceID = TransBirForms.InvoiceID" &
            "   ORDER BY tbl2.DateCreated DESC" &
            ") AS BirFormUserID" &
            " FROM TransBirForms" & vbCrLf & _
            " JOIN TransInvoice ON TransInvoice.ID = TransBirForms.InvoiceID" & vbCrLf & _
            " WHERE TransInvoice.CustomerID = " & ReadyString(vID) & vbCrLf & _
            " GROUP BY TransBirForms.InvoiceID" & vbCrLf

        pSql = "SELECT *" &
            ",tblEmp.LastName + ', ' + tblEmp.FirstName + ' ' + tblEmp.MiddleName AS BirFormFullName" &
            " FROM (" & pSql & ")t" &
            " LEFT JOIN DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmp ON tblEmp.ID = t.BirFormUserID"

        Return mProData.Execute(pSql)

    End Function

    Public Function GetInvoicesAndForm2307History(vID As String, Optional ByVal vWithBalanceOnly As Boolean = True) As DataTable

        Dim pDtInvoices As DataTable
        Dim pDtFormHistory As DataTable
        Dim pDrFormHistory As DataRow
        Dim pDRs As DataRow()
        Dim pDR As DataRow

        pDtInvoices = GetInvoices(vID, vWithBalanceOnly)
        pDtInvoices.Columns.Add("BirFormDateCreatedMax", GetType(Date))
        pDtInvoices.Columns.Add("BirFormDateCreatedMin", GetType(Date))
        pDtInvoices.Columns.Add("BirFormCount", GetType(Integer))
        pDtInvoices.Columns.Add("BirFormUserID", GetType(Integer))
        pDtInvoices.Columns.Add("BirFormFullName")

        pDtFormHistory = GetForm2307History(vID)
        For Each pDrFormHistory In pDtFormHistory.Rows
            pDRs = pDtInvoices.Select("InvoiceID = " & ReadyString(pDrFormHistory("InvoiceID")))
            For Each pDR In pDRs
                pDR("BirFormDateCreatedMax") = pDrFormHistory("BirFormDateCreatedMax")
                pDR("BirFormDateCreatedMin") = pDrFormHistory("BirFormDateCreatedMin")
                pDR("BirFormCount") = pDrFormHistory("BirFormCount")
                pDR("BirFormUserID") = pDrFormHistory("BirFormUserID")
                pDR("BirFormFullName") = pDrFormHistory("BirFormFullName")
            Next pDR
        Next pDrFormHistory

        Return pDtInvoices

    End Function

    Public Function IsZeroRated(ByVal vCustomerID As String) As Boolean

        Dim pSql As String

        pSql = "SELECT VATType" & _
            " FROM LibraryCustomers" & _
            " WHERE ID = " & ReadyString(vCustomerID)
        pSql = mProData.ExecuteScalar(pSql)
        Return CStr2(pSql).ToLower = "Zero Rated Sales/Receipts".ToLower

    End Function

    Public Function RecentlyBought(vCustomerID As String, vStartDate As Date) As DataTable

        Dim pDT As DataTable
        Dim pFinder As Finder

        pFinder = New Finder
        pFinder.MaxRecords = 50
        pFinder.ProData = mProData

        With pFinder
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                    .Expression = "MobizLibraryPIMProduct.ID"
                End With

                With .Add("ProductNo", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductNo"
                End With

                With .Add("ProductName", SqlDbType.VarChar)
                    .Expression = "MobizLibraryPIMProduct.ProductName" & vbCrLf
                End With
                With .Add("InvoiceDate", SqlDbType.DateTime2)
                    .Expression = "MAX(TransInvoice.InvoiceDate)"
                    .Searchable = False
                End With

                With .Add("SubTotal", SqlDbType.Money)
                    .Expression = "SUM(TransInvoiceItems.SubTotal)"
                End With
            End With
            .SqlFrom = " FROM TransInvoice" & vbCrLf & _
                " JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID" & vbCrLf & _
                " JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = TransInvoiceItems.ProductID" & vbCrLf
            .SqlWhere = "WHERE TransInvoice.CustomerID = " & ReadyString(vCustomerID) & vbCrLf &
                " AND TransInvoice.InvoiceDate >= " & ReadyString(vStartDate)
            .SqlGroup = " GROUP BY MobizLibraryPIMProduct.ID" & vbCrLf & _
                " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
                " ,MobizLibraryPIMProduct.ProductName" & vbCrLf
            .SqlOrder = " ORDER BY MAX(TransInvoice.InvoiceDate) DESC" & vbCrLf & _
                " ,MobizLibraryPIMProduct.ProductNo" & vbCrLf & _
                " ,MobizLibraryPIMProduct.ProductName" & vbCrLf
            pDT = .Requery
        End With

        Return pDT

    End Function

    'Public Function GetCart(vID As String, Optional ByRef rSalesOrderID As String = Nothing) As DataSet

    '    Dim pDS As DataSet
    '    Dim pDT As DataTable
    '    Dim pSql As String
    '    Dim pOrderID As String ' TransSalesOrder.ID

    '    If CStr2(rSalesOrderID).Length > 0 Then
    '        pSql = " AND TransSalesOrder.ID = " & ReadyString(rSalesOrderID) & vbCrLf
    '    Else
    '        pSql = " AND EXISTS(SELECT TransSalesOrderItems.ID" & vbCrLf & _
    '        " 	FROM TransSalesOrderItems" & vbCrLf & _
    '        "   	WHERE TransSalesOrderItems.SalesOrderID = TransSalesOrder.ID)" & vbCrLf
    '    End If
    '    pSql = "SELECT TOP 1 TransSalesOrder.ID" & vbCrLf & _
    '        ",TransSalesOrder.OrderNo" & vbCrLf &
    '        ",TransSalesOrder.Remarks" & vbCrLf &
    '        ",TransSalesOrder.LastUpdated" & vbCrLf &
    '        " FROM TransSalesOrder" & vbCrLf & _
    '        " WHERE TransSalesOrder.IsCancelled = 0" & vbCrLf & _
    '        " AND TransSalesOrder.CustomerID = " & ReadyString(vID) & vbCrLf & _
    '        " AND TransSalesOrder.IsHold = 1" & vbCrLf & _
    '        pSql &
    '        " ORDER BY TransSalesOrder.LastUpdated DESC" & vbCrLf
    '    pDT = mProData.Execute(pSql)
    '    If pDT.Rows.Count > 0 Then
    '        pDS = New DataSet
    '        pDT.TableName = "Master"
    '        pDS.Tables.Add(pDT)
    '        pOrderID = CStr2(pDT.Rows(0)("ID"))
    '        rSalesOrderID = pOrderID

    '        pDT = mParent.TransSalesOrder.GetSalesOrderItems(pOrderID)
    '        pDT.TableName = "Stocks"
    '        pDS.Tables.Add(pDT)
    '    Else
    '        Return Nothing
    '    End If

    '    Return pDS

    'End Function

    Public Function GetCartOrderID(vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 TransSalesOrder.ID" & vbCrLf & _
            " FROM TransSalesOrder" & vbCrLf & _
            " WHERE TransSalesOrder.IsCancelled = 0" & vbCrLf & _
            " AND TransSalesOrder.IsHold = 1" & vbCrLf & _
            " AND TransSalesOrder.CustomerID = " & ReadyString(vID) & vbCrLf & _
            " ORDER BY TransSalesOrder.LastUpdated DESC" & vbCrLf
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0)("ID"))
        End If

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vFowstID">MobizLibraryOrganizationFOWST.ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByFowstID(vFowstID As String) As DataRow

        Dim pDR As DataRow
        Dim pSql As String

        pSql = "SELECT MobizLibraryOrganizationFOWST.ID" &
            ",MobizLibraryOrganizationFOWST.Name" & vbCrLf & _
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf & _
            " WHERE MobizLibraryOrganizationFOWST.ID = " & ReadyString(vFowstID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    Public Function LegacyGetOrAddContactPerson(vID As String) As String

        Dim pOut As String

        pOut = LegacyGetContactPerson(vID)
        If CStr2(pOut) = "" Then
            pOut = LegacyCreateContactPerson(vID)
        End If

        Return pOut

    End Function

    Public Function LegacyGetCustomerContactPerson(ByVal vID As String) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 ID, (LibraryCustomersRepresentatives.LastName + ', ' + LibraryCustomersRepresentatives.FirstName + ' ' + LibraryCustomersRepresentatives.MiddleName) AS ContactPerson " & _
            " FROM LibraryCustomersRepresentatives " & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY IsDefaultRepresentative DESC" & _
            ",LastName, FirstName, MiddleName"
        pDT = mProData.Execute(pSql)

        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If
        pDT.Dispose()

    End Function

    Public Function LegacyGetContactPerson(ByVal vID As String) As String

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT TOP 1 ID, (LibraryCustomersRepresentatives.LastName + ', ' + LibraryCustomersRepresentatives.FirstName + ' ' + LibraryCustomersRepresentatives.MiddleName) AS ContactPerson " & _
            " FROM LibraryCustomersRepresentatives " & _
            " WHERE CustomerID = " & ReadyString(vID) & _
            " ORDER BY IsDefaultRepresentative DESC" & _
            ",LastName, FirstName, MiddleName"
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return CStr2(pDT.Rows(0)("ID"))
        End If
        pDT.Dispose()

    End Function

    ''' <summary>
    ''' return LibraryCustomersRepresentatives.ID
    ''' </summary>
    ''' <param name="vID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function LegacyCreateContactPerson(vID As String) As String

        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pID As String

        pID = Guid.NewGuid.ToString
        pIU = New InsertUpdate("LibraryCustomersRepresentatives")
        pIU("ID") = ReadyString(pID)
        pIU("CustomerID") = ReadyString(vID)
        pSql = pIU.GetInsert
        mProData.ExecuteSQL(pSql)

        Return pID

    End Function

    ''' <summary>
    ''' requires private database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DailyCustomerInvoice() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TransInvoice.CustomerID" & vbCrLf &
            " ,SUM(TransInvoice.TotalAmount) AS TotalAmount" & vbCrLf &
            " ,CONVERT(date, TransInvoice.InvoiceDate) AS InvoiceDate" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " GROUP BY TransInvoice.CustomerID" & vbCrLf &
            " ,CONVERT(date, TransInvoice.InvoiceDate)" & vbCrLf &
            " ORDER BY InvoiceDate" & vbCrLf &
            " ,CustomerID"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' requires private database
    ''' </summary>
    Public Function SaveDailyCustomerInvoice() As Integer

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pDT = DailyCustomerInvoice()
        pIU = New InsertUpdate("CacheDailyCustomerInvoice")
        For Each pDR As DataRow In pDT.Rows
            pIU("Amount") = pDR("TotalAmount")
            pSql = pIU.GetUpdate("CustomerID = " & ReadyString(pDR("CustomerID")) &
                " AND InvoiceDate = " & ReadyString(pDR("InvoiceDate")))
            pCount = mProData.ExecuteSQL(pSql)
            If pCount = 0 Then
                pIU("CustomerID") = ReadyString(pDR("CustomerID"))
                pIU("InvoiceDate") = ReadyString(pDR("InvoiceDate"))
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            End If
        Next pDR

        Return pDT.Rows.Count

    End Function

    ''' <summary>
    ''' requires private database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MonthlyCustomerInvoice() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = " SELECT TransInvoice.CustomerID" & vbCrLf &
            " ,SUM(TransInvoice.TotalAmount) AS TotalAmount" & vbCrLf &
            " ,DATEPART(YEAR, TransInvoice.InvoiceDate) AS InvoiceYear" & vbCrLf &
            " ,DATEPART(MONTH, TransInvoice.InvoiceDate) AS InvoiceMonth" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " WHERE TransInvoice.InvoiceNumber > 0" & vbCrLf &
            " GROUP BY TransInvoice.CustomerID" & vbCrLf &
            " ,DATEPART(YEAR, TransInvoice.InvoiceDate)" & vbCrLf &
            " ,DATEPART(MONTH, TransInvoice.InvoiceDate)" & vbCrLf &
            " ORDER BY InvoiceYear" & vbCrLf &
            " ,InvoiceMonth" & vbCrLf &
            " ,CustomerID"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' requires private database
    ''' </summary>
    Public Function SaveMonthlyCustomerInvoice() As Integer

        Dim pDT As DataTable
        Dim pIU As InsertUpdate
        Dim pSql As String
        Dim pCount As Integer

        pDT = MonthlyCustomerInvoice()
        pIU = New InsertUpdate("CacheMonthlyCustomerInvoice")
        For Each pDR As DataRow In pDT.Rows
            pIU("Amount") = pDR("TotalAmount")
            pIU("LastUpdated") = "GETDATE()"
            pSql = pIU.GetUpdate("CustomerID = " & ReadyString(pDR("CustomerID")) &
                " AND InvoiceYear = " & pDR("InvoiceYear") &
                " AND InvoiceMonth = " & pDR("InvoiceMonth")
            )
            pCount = mProData.ExecuteSQL(pSql)
            If pCount = 0 Then
                pIU("CustomerID") = ReadyString(pDR("CustomerID"))
                pIU("InvoiceYear") = pDR("InvoiceYear")
                pIU("InvoiceMonth") = pDR("InvoiceMonth")
                pSql = pIU.GetInsert
                mProData.ExecuteSQL(pSql)
            End If
        Next pDR

        Return pDT.Rows.Count

    End Function

    Public Function GetUnappliedPayments(vOrgID As String) As Decimal

        Dim pOut As Decimal
        Dim pSql As String

        pSql = " SELECT SUM(TransCustomerPayment.UnappliedPayment - ISNULL(TransCustomerPayment.ConsumedUnappliedPayment, 0)) AS UnappliedPayment" & vbCrLf &
            " FROM TransCustomerPayment" & vbCrLf &
            " WHERE TransCustomerPayment.IsCancelled = 0" & vbCrLf &
            " AND TransCustomerPayment.CustomerID = " & ReadyString(vOrgID)
        pOut = CDec2(mProData.ExecuteScalar(pSql))

        pSql = "SELECT SUM(TransInvoiceAdjustments.UnappliedCredits)" &
            " FROM TransInvoiceAdjustments" & vbCrLf &
            " JOIN TransInvoice ON TransInvoice.ID = TransInvoiceAdjustments.InvoiceID" & vbCrLf &
            " WHERE TransInvoice.CustomerID = " & ReadyString(vOrgID) &
            " AND IsActive = 1"
        pOut += CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function GetExcessPayments(vOrgID As String) As Decimal

        Dim pOut As Decimal
        Dim pSql As String

        pSql = " SELECT SUM(TotalPayment - NetInvoiceAmount)" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE CacheInvoices.CustomerID = " & ReadyString(vOrgID) & vbCrLf &
            " AND NetInvoiceAmount < TotalPayment"
        pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pOut

    End Function

    Public Function GetData(vOrgID As String) As DataRow

        Dim pSql As String
        Dim pDR As DataRow

        pSql = " SELECT LibraryOrganizationCustomer.CreditBegBalance" & vbCrLf &
            " ,LibraryOrganizationCustomer.CreditBalanceAsOf" & vbCrLf &
            " ,LibraryOrganizationCustomer.CreditLimit" & vbCrLf &
            " ,LibraryPaymentTerms.Days AS CreditTerm" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " LEFT JOIN LibraryPaymentTerms ON LibraryPaymentTerms.ID = LibraryOrganizationCustomer.CreditTermsID" & vbCrLf &
            " WHERE LibraryOrganizationCustomer.OrganizationID = " & vbCrLf & ReadyString(vOrgID)
        pDR = mProData.ExecuteRow(pSql)

        Return pDR

    End Function

    ''' <summary>
    ''' returns the base filename of the created PDF file
    ''' </summary>
    ''' <param name="vOrgID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateBilling(vOrgID As String, _
        Optional vEarliestDate As Date? = Nothing) As String

        Dim pReport As ProReport
        Dim pDtInvoices As DataTable
        Dim pFilename As String
        Dim pBaseFilename As String
        Dim pHeaderInfo As LibraryOrganization.HeaderInfo

        pReport = New ProReport()
        pReport.AutoFillHeader = False
        pReport.SourceFilename = "Billing"
        pDtInvoices = mParent.TransInvoice.GetInvoicesWithCreditFromCache(vOrgID, , , , , True, vEarliestDate)
        pDtInvoices.Columns.Add("PoNumText")
        For Each pDrInvoice As DataRow In pDtInvoices.Rows
            If CStr2(pDrInvoice("PurchaseOrderText")).Trim() = "" Then
                If CStr2(pDrInvoice("FullSalesOrderNum")).Trim() <> "" Then
                    pDrInvoice("PoNumText") = "SO " & CStr2(pDrInvoice("FullSalesOrderNum")).Trim() & " - " & Format(CDateNull(pDrInvoice("SalesOrderDate")), cGlobal.FormatDateGrid)
                End If
            Else
                pDrInvoice("PoNumText") = "PO " & CStr2(pDrInvoice("PurchaseOrderText")).Trim() & " - " & Format(CDateNull(pDrInvoice("PurchaseOrderDate")), cGlobal.FormatDateGrid)
            End If
        Next
        pDtInvoices.DefaultView.Sort = "InvoiceDueDate"
        pReport.AddView("Invoices", pDtInvoices.DefaultView)
        pReport.Variables("StatementDate") = "Statement Date: " & Format(Now, "MM/dd/yyyy hh:mm tt")

        pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(WebRequest.Instance.OrgID)
        pReport.Variables("CompanyName") = pHeaderInfo.CompanyName
        pReport.Variables("CompanyAddress") = pHeaderInfo.CompanyAddress
        pReport.Variables("CompanyContactInfos") = pHeaderInfo.ContactInfos

        pHeaderInfo = mParent.LibraryOrganization.GetHeaderInfo(vOrgID)
        pReport.Variables("CustomerName") = pHeaderInfo.CompanyName
        pReport.Variables("CustomerAddress") = pHeaderInfo.CompanyAddress
        pReport.Variables("CustomerContactInfos") = pHeaderInfo.ContactInfos
        pReport.Variables("Footnote") = CStr2(mParent.SystemDefaults.SOAFootnote)

        Dim pDrCustomer = GetData(vOrgID)
        If pDrCustomer IsNot Nothing Then
            pReport.Variables("CreditBegBalance") = CDec2(pDrCustomer("CreditBegBalance"))
        End If

#If DEBUG Then
        ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\Stimulsoft XML Data.xml"
        pReport.ExportXmlFile()
#End If
        pBaseFilename = GenerateString(20)
        pFilename = Common.ServerPath & "temp\Billing\" & pBaseFilename & ".pdf"
        pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

        Return pBaseFilename

    End Function

    Public Function ComputeCnCUnapprovedReason(vOrgID As String, vAmountForApproval As Decimal) As String

        Dim pSql As String
        Dim pNetInvoiceBalance As Decimal
        Dim pCreditLimit As Decimal

        pSql = "SELECT LibraryOrganizationCustomer.CreditLimit" & vbCrLf &
            ",LibraryOrganizationCustomer.UnlimitedCredit" & vbCrLf &
            ",LibraryOrganizationCustomer.RequireCnCApproval" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " WHERE LibraryOrganizationCustomer.OrganizationID = " & ReadyString(vOrgID)
        Dim pDR = mProData.ExecuteRow(pSql)
        If pDR Is Nothing Then
            Return Nothing
        End If
        
        If CBool2(pDR("RequireCnCApproval")) Then
            Return "Customer Requires Approval"
        End If

        If CBool2(pDR("UnlimitedCredit")) Then
            Return Nothing
        End If

        pCreditLimit = CDec2(pDR("CreditLimit"))

        pSql = "SELECT SUM(NetInvoiceBalance)" & vbCrLf &
            " FROM CacheInvoices" & vbCrLf &
            " WHERE CustomerID = " & ReadyString(vOrgID)
        Try
            pNetInvoiceBalance = CDec2(mProData.ExecuteScalar(pSql))
        Catch ex As Exception
            AppendToLog("vbGQ9XGPB4N9! " & ex.Message & vbCrLf &
                pSql, , "timeout")
            Throw
        End Try
        If pCreditLimit < pNetInvoiceBalance + vAmountForApproval Then
            Return "Over Credit Limit"
        End If


    End Function

    Public Function GetPriceHistory(vOrgID As String, vProductID As String) As DataTable

        Dim pOut As DataTable
        Dim pSql As String

        pSql = " -- price history" & vbCrLf &
            " SELECT TransInvoiceItems.ID" & vbCrLf &
            "  ,TransInvoice.InvoiceDate" & vbCrLf &
            "  ,TransInvoiceItems.Quantity" & vbCrLf &
            "  ,TransInvoiceItems.HiddenServedQuantity" & vbCrLf &
            "  ,TransInvoiceItems.OrderedQuantity" & vbCrLf &
            "  ,TransInvoiceItems.UnitMeasure" & vbCrLf &
            "  ,TransInvoiceItems.UnitPrice" & vbCrLf &
            "  ,TransInvoiceItems.SubTotal" & vbCrLf &
            "  ,TransInvoiceItems.NetDue" & vbCrLf &
            "  ,TransInvoiceItems.TermNumericValue" & vbCrLf &
            "  ,TransInvoiceItems.ActualQuantity" & vbCrLf &
            "  ,TransInvoiceItems.InvoiceID" & vbCrLf &
            "  ,TransInvoiceItems.LineNumber" & vbCrLf &
            "  ,TransInvoiceItems.PriceLevelID" & vbCrLf &
            " ,tblBaseUnit.UnitMeasure AS BaseUnit" & vbCrLf &
            " ,tblBaseUnit.UnitQty AS BaseUnitQty" & vbCrLf &
            " ,ISNULL(CONVERT(varchar, MobizLibraryOrganizationFOWST.FOWSTNo), '0') + '-' + ISNULL(CONVERT(varchar, LibraryDevice.DeviceNo), '0') + '-' + ISNULL(CONVERT(varchar, TransInvoice.InvoiceNumber), '0') AS InvoiceNum" & vbCrLf &
            "  ,MobizLibraryPIMPriceLevel.PriceLevel AS PriceLevelName" & vbCrLf &
            "  FROM TransInvoiceItems" & vbCrLf &
            "  JOIN TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID" & vbCrLf &
            "  LEFT JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.Label = TransInvoiceItems.UnitMeasure" & vbCrLf &
            "  LEFT JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.MeasureNameID = MobizLibraryCountriesLanguageLabel.ID" & vbCrLf &
            "  OUTER APPLY (" & vbCrLf &
            "  SELECT TOP 1 MobizLibraryCountriesLanguageLabel.Label AS UnitMeasure" & vbCrLf &
            "  	,MobizLibraryPIMProductMeasurement.Quantity AS UnitQty" & vbCrLf &
            "      FROM MobizLibraryPIMProductMeasurement" & vbCrLf &
            "      JOIN MobizLibraryPIMMeasure ON MobizLibraryPIMMeasure.ID = MobizLibraryPIMProductMeasurement.UnitMeasureID" & vbCrLf &
            "      JOIN MobizLibraryCountriesLanguageLabel ON MobizLibraryCountriesLanguageLabel.ID = MobizLibraryPIMMeasure.MeasureNameID" & vbCrLf &
            "      WHERE MobizLibraryPIMProductMeasurement.ProductID = TransInvoiceItems.ProductID" & vbCrLf &
            "      ORDER BY MobizLibraryPIMProductMeasurement.Quantity" & vbCrLf &
            "  ) tblBaseUnit" & vbCrLf &
            "  LEFT JOIN LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID" & vbCrLf &
            "  LEFT JOIN MobizLibraryOrganizationFOWST ON TransInvoice.FowstID = MobizLibraryOrganizationFOWST.ID" & vbCrLf &
            "  LEFT JOIN MobizLibraryPIMPriceLevel ON MobizLibraryPIMPriceLevel.ID = TransInvoiceItems.PriceLevelID" & vbCrLf &
            "  WHERE TransInvoice.CustomerID = " & ReadyString(vOrgID) & vbCrLf &
            "  AND TransInvoiceItems.ProductID = " & ReadyString(vProductID) & vbCrLf &
            "  AND (TransInvoiceItems.Quantity > 0)" & vbCrLf &
            "  AND TransInvoice.InvoiceNumber > 0" & vbCrLf &
            "  ORDER BY TransInvoice.InvoiceDate DESC"
        pOut = mProData.Execute(pSql)

        Return pOut

    End Function

    Public Function UpdateBalanceFromDuc(Optional vCustomerOrgID As String = Nothing, Optional vForce As Boolean = False) As DataTable

        Dim pProData As ProData
        Dim pCI = New ConnectionInfo()

        pCI.Server = Common.ConnectionInfo.Server
        pCI.Database = "Wholesale"
        pCI.Username = Common.ConnectionInfo.Username
        pCI.Password = Common.ConnectionInfo.Password
        pCI.IntegratedSecurity = Common.ConnectionInfo.IntegratedSecurity
        pProData = ProData.CreateFromConnectionInfo(pCI)

        Dim pLastUpdated = GetLastUpdateForGetBalanceFromDuc(pProData)
        If vForce = False Then
            Dim pLastCache = mParent.SystemDefaults.LastUpdateForGetBalanceFromDuc
            If pLastCache IsNot Nothing Then
                If pLastUpdated <= pLastCache Then
                    Return Nothing
                End If
            End If
        End If

        Dim pDT = GetBalanceFromDuc(pProData, vCustomerOrgID)
        Dim pCount As Integer
        For Each pDR As DataRow In pDT.Rows
            Dim pIU = New InsertUpdate("LibraryOrganizationCustomer")
            pIU("ThirdPartyBegBalance") = CDec2(pDR("Balance"))
            pIU("ThirdPartyBegBalanceAsOf") = "GETDATE()"
            Dim pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(pDR("CustomerID")))
            pCount += mProData.ExecuteSQL(pSql)
        Next pDR

        mParent.SystemDefaults.LastUpdateForGetBalanceFromDuc = pLastUpdated

    End Function

    Private Function GetBalanceFromDuc(ByRef rProData As ProData, Optional vCustomerOrgID As String = Nothing) As DataTable

        Dim pWhere = ""

        If vCustomerOrgID IsNot Nothing Then
            pWhere = " AND CustomerID = " & ReadyString(vCustomerOrgID) & vbCrLf
        End If

        Dim pSql = " -- new query for GetBalanceFromDuc" & vbCrLf &
            " SELECT LibraryCustomers.ID AS CustomerID" & vbCrLf &
            " ,t.CustomerName" & vbCrLf &
            " ,SUM(Balance) AS Balance" & vbCrLf &
            " FROM LibraryCustomers" & vbCrLf &
            " LEFT JOIN (" & vbCrLf &
            "   SELECT CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            "   ,SUM(Balance) AS Balance" & vbCrLf &
            "   FROM vwReportCustomerTAccount" & vbCrLf &
            "   WHERE Balance >= 0" & vbCrLf &
            pWhere &
            "   GROUP BY CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            "   UNION" & vbCrLf &
            "   SELECT CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            "   ,SUM(Balance) AS Balance" & vbCrLf &
            "   FROM vwReportCustomerTAccount1" & vbCrLf &
            "   WHERE Balance >= 0" & vbCrLf &
            pWhere &
            "   GROUP BY CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            " ) t ON t.CustomerID = LibraryCustomers.ID" & vbCrLf &
            " GROUP BY LibraryCustomers.ID, CustomerName" & vbCrLf &
            " ORDER BY CustomerName"
        Dim pDT = rProData.Execute(pSql)

        Return pDT

    End Function

    Private Function GetLastUpdateForGetBalanceFromDuc(ByRef rProData As ProData) As Date

        Dim pTableNames As New List(Of String)

        pTableNames.Add("TransInvoice")
        pTableNames.Add("TransMemo")
        pTableNames.Add("TransCustomerPayment")

        Dim pSql As String

        For Each pTableName As String In pTableNames
            If String.IsNullOrWhiteSpace(pSql) = False Then
                pSql &= "UNION" & vbCrLf
            End If
            pSql &= "SELECT LastUpdated" & vbCrLf &
                " FROM " & EscapeSqlName(pTableName) & vbCrLf
        Next pTableName

        pSql = "SELECT TOP 1 * FROM (" & vbCrLf & pSql
        pSql &= ") t" & vbCrLf &
            " ORDER BY LastUpdated DESC"
        rProData.CommandTimeout = 10 * 60
        Dim pOut = rProData.ExecuteScalar(pSql)

        Return pOut

    End Function

    Public Function DucThirdPartyBegBalanceChecksum() As Decimal

        Dim pSql = " -- DUC ThirdPartyBegBalance checksum" & vbCrLf &
            " -- DucThirdPartyBegBalanceChecksum" & vbCrLf &
            " WITH CTE AS (" & vbCrLf &
            " SELECT wholesale.dbo.LibraryCustomers.ID AS CustomerID" & vbCrLf &
            " ,t.CustomerName" & vbCrLf &
            " ,SUM(Balance) AS Balance" & vbCrLf &
            " ,t2.ThirdPartyBegBalance" & vbCrLf &
            " FROM wholesale.dbo.LibraryCustomers" & vbCrLf &
            " LEFT JOIN (" & vbCrLf &
            "   SELECT CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            "   ,SUM(Balance) AS Balance" & vbCrLf &
            "   FROM wholesale.dbo.vwReportCustomerTAccount" & vbCrLf &
            " --  where customerid  = 'BDA2905D-9D70-4EE7-B06C-19799D10D9EC'" & vbCrLf &
            "   WHERE Balance >= 0" & vbCrLf &
            "   GROUP BY CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            "   UNION" & vbCrLf &
            "   SELECT CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            "   ,SUM(Balance) AS Balance" & vbCrLf &
            "   FROM wholesale.dbo.vwReportCustomerTAccount1" & vbCrLf &
            " --  where customerid  = 'BDA2905D-9D70-4EE7-B06C-19799D10D9EC'" & vbCrLf &
            "   WHERE Balance >= 0" & vbCrLf &
            "   GROUP BY CustomerID" & vbCrLf &
            "   ,CustomerName" & vbCrLf &
            " ) t ON t.CustomerID = LibraryCustomers.ID" & vbCrLf &
            " CROSS APPLY (" & vbCrLf &
            " 	SELECT ThirdPartyBegBalance" & vbCrLf &
            "     ,LibraryOrganizationCustomer.OrganizationID" & vbCrLf &
            "     FROM LibraryOrganizationCustomer" & vbCrLf &
            "     WHERE LibraryOrganizationCustomer.OrganizationID = LibraryCustomers.ID" & vbCrLf &
            " ) t2" & vbCrLf &
            " GROUP BY LibraryCustomers.ID, CustomerName" & vbCrLf &
            " ,t2.ThirdPartyBegBalance" & vbCrLf &
            " )" & vbCrLf &
            " SELECT SUM(ISNULL(ThirdPartyBegBalance,0) - ISNULL(Balance, 0)) AS Diff" & vbCrLf &
            " FROM CTE"
        mProData.CommandTimeout = 10 * 60
        Dim pOut = CDec2(mProData.ExecuteScalar(pSql))

        Return pout

    End Function

    Public Function GetFinancialDetails(vCustomerOrgID As String, vOrgID As String, vIncludeDeactivated As Boolean) As DataTable

        Dim pCustomerSelect, pCustomerFrom As String

        BrowserController.ComposeCustomer(pCustomerSelect, pCustomerFrom)
        Dim pSql As String
        If String.IsNullOrWhiteSpace(vOrgID) = False Then
            pSql = ", " & ReadyString(vOrgID)
        End If
        pSql = "SELECT LibraryOrganizationFinancialDetails.ID" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.BankOrgID" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.BankBranch" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.AccountName" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.AccountNumber" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.OnlinePayment" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.CustomerOrgID" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.DeactivatedByID" & vbCrLf &
            ",LibraryOrganizationFinancialDetails.DeactivatedDate" & vbCrLf &
            "," & pCustomerSelect & " AS BankName" & vbCrLf &
            " FROM LibraryOrganizationFinancialDetails" & vbCrLf &
            " JOIN MobizLibraryOrganization ON LibraryOrganizationFinancialDetails.BankOrgID = MobizLibraryOrganization.ID " & vbCrLf & _
            pCustomerFrom &
            " WHERE LibraryOrganizationFinancialDetails.CustomerOrgID IN (" & ReadyString(vCustomerOrgID) & pSql & ")" & vbCrLf
        If vIncludeDeactivated = False Then
            pSql &= " AND LibraryOrganizationFinancialDetails.DeactivatedByID IS NULL" & vbCrLf
        End If
        pSql &= " ORDER BY BankName, BankBranch"
        Dim pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Sub Deactivate(vOrgID As String, vReason As String)

        Dim pIU = New InsertUpdate("LibraryOrganizationCustomer")
        pIU("DeactivationReason") = ReadyString(vReason)
        pIU("Deactivated") = 1
        pIU("DeactivationDate") = "GETDATE()"
        Dim pSql = pIU.GetUpdate("OrganizationID = " & ReadyString(vOrgID))
        mProData.ExecuteSQL(pSql)

    End Sub

    Public Sub TransferInvoices(vOldCustomerID As String, vNewCustomerID As String)

        Dim pIU = New InsertUpdate("TransInvoice")
        pIU("CustomerID") = ReadyString(vNewCustomerID)
        Dim pSql = pIU.GetUpdate("CustomerID = " & ReadyString(vOldCustomerID))
        pSql = mProData.ExecuteSQL(pSql) ' output is used to debugging only

    End Sub

    Public Function GetCreditLimit(vCustomervOrgID As String) As Decimal

        Dim pSql = "SELECT LibraryOrganizationCustomer.CreditLimit" & vbCrLf &
            ",LibraryOrganizationCustomer.UnlimitedCredit" & vbCrLf &
            " FROM LibraryOrganizationCustomer" & vbCrLf &
            " WHERE LibraryOrganizationCustomer.OrganizationID = " & ReadyString(vCustomervOrgID)
        Try
            
            Dim pDR = mProData.ExecuteRow(pSql)
            If pDR Is Nothing Then
                Return 0
            End If

            If CBool2(pDR("UnlimitedCredit")) Then
                Return Decimal.MaxValue
            End If

            Return CDec2(pDR("CreditLimit"))
        Catch ex As Exception
            AppendToLog("vbUDJS2SJTNY! " & ex.Message & vbCrLf &
                pSql, , "timeout")
            Throw
        End Try

    End Function

    Public Sub SetAllCustomerUnlimitedCredit(vValue As Boolean)

        Dim pIU = New InsertUpdate("LibraryOrganizationCustomer")
        pIU("UnlimitedCredit") = If(vValue, 1, 0)
        Dim pSql = pIU.GetUpdate("1=1")
        pSql = mProData.ExecuteSQL(pSql) ' output is used to debugging only

    End Sub
End Class

Public Class EmployeeProfiles

    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetAgents() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT DISTINCT EmployeeProfiles.ID" &
            ",EmployeeProfiles.LastName + ', ' + EmployeeProfiles.FirstName + ' ' + EmployeeProfiles.MiddleName AS FullName" &
            ",EmployeeProfiles.EmailAddress" &
            ",tblEmployees.LastName + ', ' + tblEmployees.FirstName + ' ' + tblEmployees.MiddleName AS UpdatedByName" &
            ",EmployeeProfiles.Stamp" &
            " FROM DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles" &
            " JOIN LibraryCustomersAgents ON LibraryCustomersAgents.EmployeeID = EmployeeProfiles.ID" &
            " LEFT JOIN  DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmployees ON tblEmployees.ID = EmployeeProfiles.RegisteredBy" &
            " WHERE EmployeeProfiles.IsActive = 1" &
            " ORDER BY EmployeeProfiles.LastName + ', ' + EmployeeProfiles.FirstName + ' ' + EmployeeProfiles.MiddleName"
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function

    Public Function ById(vID As Integer) As DataRow

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT EmployeeProfiles.ID" &
            ",EmployeeProfiles.FirstName" &
            ",EmployeeProfiles.LastName" &
            ",EmployeeProfiles.LastName + ', ' + EmployeeProfiles.FirstName + ' ' + EmployeeProfiles.MiddleName AS FullName" &
            ",EmployeeProfiles.EmailAddress" &
            ",tblEmployees.LastName + ', ' + tblEmployees.FirstName + ' ' + tblEmployees.MiddleName AS UpdatedByName" &
            ",EmployeeProfiles.Stamp" &
            " FROM DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles" &
            " LEFT JOIN  DavaoUnifiedPayroll.dbo.EmployeeProfiles tblEmployees ON tblEmployees.ID = EmployeeProfiles.RegisteredBy" &
            " WHERE EmployeeProfiles.ID = " & vID
        pDT = mProData.Execute(pSql)
        If pDT.Rows.Count > 0 Then
            Return pDT.Rows(0)
        End If

    End Function

    Public Function GetCustomers(vID As Integer) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT CustomerID" &
            " FROM LibraryCustomersAgents" &
            " WHERE EmployeeID = " & vID
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class

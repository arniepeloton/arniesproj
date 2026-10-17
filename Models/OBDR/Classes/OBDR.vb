Public Class OBDR
    Public Function getBankInformation(ByVal BankUserAccountID As String) As DataTable
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT OBDR.dbo.LibraryBranchBankInfo.ID" & vbCrLf &
                   ", LibraryBanks.BankName" & vbCrLf &
                   ", OBDR.dbo.LibraryUserAccounts.FirstName" & vbCrLf &
                   "FROM OBDR.dbo.LibraryBranchBankInfo INNER JOIN" & vbCrLf &
                   "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryBranchBankInfo.BranchID INNER JOIN" & vbCrLf &
                   "LibraryBanks ON LibraryBanks.ID = OBDR.dbo.LibraryBranchBankInfo.BankID INNER JOIN" & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts ON OBDR.dbo.LibraryUserAccounts.BankID = OBDR.dbo.LibraryBranchBankInfo.BankID" & vbCrLf &
                   "WHERE OBDR.dbo.LibraryBranchBankInfo.IsDepositoryBank = 1 AND OBDR.dbo.LibraryUserAccounts.ID = " & ReadyString(BankUserAccountID)
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function getBankAccountNumber(ByVal BankUserAccountID As String, ByVal BranchID As Integer) As String
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT * FROM OBDR.dbo.LibraryBranchBankInfo INNER JOIN" & vbCrLf &
                   "LibraryBanks ON LibraryBanks.ID = LibraryBankBranch.BankID INNER JOIN" & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts ON OBDR.dbo.LibraryUserAccounts.BankID = LibraryBanks.ID" & vbCrLf &
                   "WHERE OBDR.dbo.LibraryBranchBankInfo.IsDepositoryBank = 1 AND OBDR.dbo.LibraryUserAccounts.ID = " & ReadyString(BankUserAccountID) & vbCrLf &
                   "OBDR.dbo.LibraryBranchBankInfo = " & BranchID
    End Function

    Public Function getAllBranchesBasedOnBankUserAccounts(ByVal BankUserAccountID As String) As DataTable
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT DavaoUnifiedPayroll.dbo.LibraryBranch.ID" & vbCrLf &
                   ", DavaoUnifiedPayroll.dbo.LibraryBranch.Name" & vbCrLf &
                   "FROM OBDR.dbo.LibraryBranchBankInfo INNER JOIN" & vbCrLf &
                   "LibraryBanks ON LibraryBanks.ID = OBDR.dbo.LibraryBranchBankInfo.BankID INNER JOIN" & vbCrLf &
                   "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryBranchBankInfo.BranchID INNER JOIN" & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts ON OBDR.dbo.LibraryUserAccounts.BankID = LibraryBanks.ID" & vbCrLf &
                   "WHERE OBDR.dbo.LibraryBranchBankInfo.IsDepositoryBank = 1 AND OBDR.dbo.LibraryUserAccounts.ID = " & ReadyString(BankUserAccountID)
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

#Region "OBDR Alerts"
    'OBDR Results Compared to End of Day Sales Reports
    Public Function GetEndOfDaySalesRecords(ByVal SalesDateStart As Date, ByVal SalesDateEnd As Date) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT ID, UserID, MachineNumber, BranchID, TotalAmount, SalesDate, LastUpdated" & vbCrLf &
                   "FROM SummaryEODS" & vbCrLf &
                   "WHERE SalesDate BETWEEN " & ReadyString(SalesDateStart) & " AND " & ReadyString(SalesDateEnd)
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords

    End Function
#End Region

#Region "OBDR Form Registration"
    Public Function ListOfBranches(ByVal Branches As String) As DataTable
        Dim sSql As String = vbNullString
        sSql = "SELECT * FROM LibraryServers" & vbCrLf &
               "WHERE BranchID " & Branches
        Return Common.WebRequest.ProData.Execute(sSql)
    End Function

    Public Function ListOfMachineNumbers(ByVal BranchID As Integer) As DataTable
        Dim sSql As String = vbNullString
        sSql = "SELECT * FROM OBDR.LibraryPOSDescriptions" & vbCrLf &
               "WHERE BranchID = " & BranchID
        Return Common.WebRequest.ProData.Execute(sSql)
    End Function

    Public Function ListAccountInfo(ByVal BranchID As Integer) As DataTable
        Dim sSql As String = vbNullString
        sSql = ""
    End Function
#End Region
End Class

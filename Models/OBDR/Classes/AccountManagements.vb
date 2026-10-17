Public Class AccountManagements

    ' 1 - No Error (Successfully Save)
    ' 0 - Error Occured by the system
    ' -1 - Duplicate Error Occured
    Public Function RegisterAccounts(ByVal inputValue As Dictionary(Of String, Object)) As Long
        Dim sqlQuery As String = vbNullString
        Dim UserID As String = Guid.NewGuid().ToString
        Dim password As String
        Dim tableRecords As New DataTable
        Dim indexCounter As Integer = 0

        sqlQuery = "INSERT INTO OBDR.dbo.LibraryUserAccounts(OBDR.dbo.LibraryUserAccounts.ID" & vbCrLf &
                   ",OBDR.dbo.LibraryUserAccounts.LastName, OBDR.dbo.LibraryUserAccounts.FirstName," & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts.MiddleName,OBDR.dbo.LibraryUserAccounts.PositionID," & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts.EmailAddress,OBDR.dbo.LibraryUserAccounts.BankID,OBDR.dbo.LibraryUserAccounts.UserID)" & vbCrLf &
                   "VALUES(" & ReadyString(UserID) & vbCrLf &
                   " , " & ReadyString(inputValue("lastname")) & vbCrLf &
                   ", " & ReadyString(inputValue("firstname")) & vbCrLf &
                   ", " & ReadyString(inputValue("middlename")) & vbCrLf &
                   ", " & ReadyString(inputValue("positionid")) & vbCrLf &
                   ", " & ReadyString(inputValue("emailaddress")) & vbCrLf &
                   ", " & ReadyString(inputValue("bankid")) & vbCrLf &
                   ",NULL)"
        With Common.WebRequest.ProData
            .BeginTransaction()
            If .ExecuteSQL(sqlQuery) = 0 Then
                .RollbackTransaction()
                Return 0
            Else
                'Save Username and Password
                CheckSecurityKey()
                password = rsaKeys.encryptMessage(inputValue("password").ToString, rsaKeys.setGetPublicKey)

                sqlQuery = "SELECT OBDR.dbo.LibraryUserAccountLogin.ID, OBDR.dbo.LibraryUserAccountLogin.Password FROM OBDR.dbo.LibraryUserAccountLogin"
                tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
                While indexCounter < tableRecords.Rows.Count
                    If rsaKeys.decryptMessage(tableRecords.Rows(indexCounter)("Password")) = inputValue("password") Then
                        Return -1
                        Exit Function
                    End If
                    indexCounter += 1
                End While

                sqlQuery = "INSERT INTO OBDR.dbo.LibraryUserAccountLogin(" & vbCrLf &
                           "OBDR.dbo.LibraryUserAccountLogin.UserAccountID, OBDR.dbo.LibraryUserAccountLogin.Username, OBDR.dbo.LibraryUserAccountLogin.Password, OBDR.dbo.LibraryUserAccountLogin.UserID)" & vbCrLf &
                           "VALUES(" & ReadyString(UserID) & vbCrLf &
                           ", " & ReadyString(inputValue("username")) & vbCrLf &
                           ", " & ReadyString(password) & vbCrLf &
                           ", NULL" & vbCrLf &
                           ")"

                If .ExecuteSQL(sqlQuery) = 0 Then
                    .RollbackTransaction()
                    Return 0
                Else
                    .CommitTransaction()
                    Return 1
                End If

            End If
        End With
    End Function

    Public Function getRegisteredAccounts() As DataTable
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT OBDR.dbo.LibraryUserAccounts.ID, OBDR.dbo.LibraryUserAccounts.LastName," & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts.FirstName, OBDR.dbo.LibraryUserAccounts.MiddleName," & vbCrLf &
                   "OBDR.dbo.LibraryUserAccounts.EmailAddress, OBDR.dbo.LibraryBankPositions.Position, " & vbCrLf &
                   "LibraryBanks.BankCode + ' - ' + LibraryBanks.BankName AS [BankName] FROM OBDR.dbo.LibraryUserAccounts" & vbCrLf &
                   "INNER JOIN OBDR.dbo.LibraryBankPositions ON OBDR.dbo.LibraryBankPositions.ID =  OBDR.dbo.LibraryUserAccounts.PositionID INNER JOIN" & vbCrLf &
                   "LibraryBanks ON LibraryBanks.ID = OBDR.dbo.LibraryUserAccounts.BankID"

        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function getLibraryBankPosition() As DataTable
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT * FROM OBDR.dbo.LibraryBankPositions"
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function getLibraryBanks() As DataTable
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT ID, BankCode + ' - ' + BankName AS [BankName] FROM LibraryBanks"
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function GetAllBranches() As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT ID, Name FROM DavaoUnifiedPayroll.dbo.LibraryBranch"
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)

        Return tableRecords
    End Function

    Public Function GetAllMachineNumberRegistered(ByVal BranchID As Integer) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT DISTINCT POSSales.MachineNumber" & vbCrLf &
                   "FROM POSSales" & vbCrLf &
                   "WHERE POSSales.BranchID = " & BranchID

        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function SavePOSDescriptions(ByVal inputValue As Dictionary(Of String, Object)) As Boolean
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT OBDR.dbo.LibraryPOSDescriptions.ID FROM OBDR.dbo.LibraryPOSDescriptions" & vbCrLf &
                   "WHERE OBDR.dbo.LibraryPOSDescriptions.BranchID = " & inputValue("branchid") & " AND OBDR.dbo.LibraryPOSDescriptions.Descriptions = " & ReadyString(inputValue("descriptions"))

        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        If tableRecords.Rows.Count = 0 Then
            'Start Inserting POS Descriptions
            sqlQuery = "INSERT INTO OBDR.dbo.LibraryPOSDescriptions(" & vbCrLf &
                       "BranchID, MachineNumber, Descriptions)" & vbCrLf &
                       "VALUES(" & inputValue("branchid") & vbCrLf &
                       ", " & ReadyString(inputValue("machinenumber")) & vbCrLf &
                       ", " & ReadyString(inputValue("descriptions")) & vbCrLf &
                       ")"
            If Common.WebRequest.ProData.ExecuteSQL(sqlQuery) = 1 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function getPOSDescriptions(Optional ByVal BranchID As Integer = 0) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        If BranchID = 0 Then
            sqlQuery = "SELECT" & vbCrLf &
                     "OBDR.dbo.LibraryPOSDescriptions.ID," & vbCrLf &
                     "OBDR.dbo.LibraryPOSDescriptions.MachineNumber," & vbCrLf &
                     "OBDR.dbo.LibraryPOSDescriptions.Descriptions," & vbCrLf &
                     "DavaoUnifiedPayroll.dbo.LibraryBranch.Name" & vbCrLf &
                     "FROM OBDR.dbo.LibraryPOSDescriptions INNER JOIN" & vbCrLf &
                     "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryPOSDescriptions.BranchID"
        Else
            sqlQuery = "SELECT" & vbCrLf &
                     "OBDR.dbo.LibraryPOSDescriptions.ID," & vbCrLf &
                     "OBDR.dbo.LibraryPOSDescriptions.MachineNumber," & vbCrLf &
                     "OBDR.dbo.LibraryPOSDescriptions.Descriptions," & vbCrLf &
                     "DavaoUnifiedPayroll.dbo.LibraryBranch.Name" & vbCrLf &
                     "FROM OBDR.dbo.LibraryPOSDescriptions INNER JOIN" & vbCrLf &
                     "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryPOSDescriptions.BranchID" & vbCrLf &
                     "WHERE OBDR.dbo.LibraryPOSDescriptions.BranchID=" & BranchID
        End If
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

#Region "Account Registered By Branch"
    Public Function getRegisteredAccountByBranch(ByVal BranchID As Integer) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT OBDR.dbo.LibraryBranchBankInfo.ID, OBDR.dbo.LibraryBranchBankInfo.AccountNumber," & vbCrLf &
                   "OBDR.dbo.LibraryBranchBankInfo.AccountName FROM OBDR.dbo.LibraryBranchBankInfo" & vbCrLf &
                   "WHERE OBDR.dbo.LibraryBranchBankInfo.BranchID = " & BranchID & vbCrLf &
                   "AND OBDR.dbo.LibraryBranchBankInfo.IsDepositoryBank = 1"
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function
#End Region

#Region "Bank Deposit Registry"
    Public Function SaveEditDepositRegistry(ByVal inputValue As Dictionary(Of String, Object), ByVal TransID As String) As Integer
        Dim sqlQuery As String = vbNullString

        sqlQuery = "INSERT INTO OBDR.dbo.TransDeposits(OBDR.dbo.TransDeposits.ID," & vbCrLf &
                    "OBDR.dbo.TransDeposits.BankInfoID, OBDR.dbo.TransDeposits.DepositAmount,OBDR.dbo.TransDeposits.SalesDate,OBDR.dbo.TransDeposits.DateDeposits," & vbCrLf &
                    "OBDR.dbo.TransDeposits.TransactionType, OBDR.dbo.TransDeposits.RovingTeller, OBDR.dbo.TransDeposits.POSDescriptionID," & vbCrLf &
                    "OBDR.dbo.TransDeposits.UserID)" & vbCrLf &
                    "VALUES(" & ReadyString(TransID) & vbCrLf &
                    ", " & ReadyString(inputValue("bankinfoid").ToString) & vbCrLf &
                    ", " & CDbl(inputValue("depositcashamount")) & vbCrLf &
                    ", " & ReadyString(inputValue("salesdate")) & vbCrLf &
                    ", " & ReadyString(inputValue("depositdate")) & vbCrLf &
                    ", " & ReadyString(inputValue("transactiontype")) & vbCrLf &
                    ", " & ReadyString(inputValue("rovingteller")) & vbCrLf &
                    ", " & ReadyString(inputValue("posdescriptionid").ToString) & vbCrLf &
                    ", NULL" & vbCrLf &
                    ")"
        Common.WebRequest.ProData.ExecuteSQL(sqlQuery)

        Return True
    End Function

    Public Function GetDepositRegistry(ByVal BankID As String, Optional ByVal IsByID As Boolean = False, Optional ByVal DepositID As String = vbNullString) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT OBDR.dbo.TransDeposits.ID" & vbCrLf &
                   ", OBDR.dbo.LibraryBranchBankInfo.AccountNumber" & vbCrLf &
                   ", OBDR.dbo.LibraryBranchBankInfo.AccountName + ' - ' + OBDR.dbo.TransDeposits.TransactionType AS [AccountName]" & vbCrLf &
                   ", DavaoUnifiedPayroll.dbo.LibraryBranch.ID AS [BranchID]" & vbCrLf &
                   ", DavaoUnifiedPayroll.dbo.LibraryBranch.Name" & vbCrLf &
                   ", OBDR.dbo.LibraryPOSDescriptions.ID AS [POSDescriptionID]" & vbCrLf &
                   ", OBDR.dbo.LibraryPOSDescriptions.Descriptions" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.DepositAmount" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.SalesDate" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.DateDeposits" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.RovingTeller" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.IsCheckPayment" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.CheckTotalAmount" & vbCrLf &
                   "FROM OBDR.dbo.TransDeposits INNER JOIN" & vbCrLf &
                   "OBDR.dbo.LibraryBranchBankInfo ON OBDR.dbo.LibraryBranchBankInfo.ID = OBDR.dbo.TransDeposits.BankInfoID INNER JOIN" & vbCrLf &
                   "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryBranchBankInfo.BranchID INNER JOIN" & vbCrLf &
                   "LibraryBanks ON LibraryBanks.ID = OBDR.dbo.LibraryBranchBankInfo.BankID INNER JOIN" & vbCrLf &
                   "OBDR.dbo.LibraryPOSDescriptions ON OBDR.dbo.LibraryPOSDescriptions.ID = OBDR.dbo.TransDeposits.POSDescriptionID" & vbCrLf &
                   "WHERE(Convert(DateTime, FLOOR(Convert(float, OBDR.dbo.TransDeposits.LastUpdated))) = Convert(DateTime, FLOOR(Convert(float, GETDATE())))) AND OBDR.dbo.TransDeposits.IsCancel = 0 "
        If IsByID = True Then
            sqlQuery &= " AND OBDR.dbo.TransDeposits.ID = " & ReadyString(DepositID.ToString)
        Else
            sqlQuery &= " AND OBDR.dbo.LibraryBranchBankInfo.BankID = " & ReadyString(BankID.ToString)
        End If

        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function deleteDepositRegistry(ByVal inputValue As Dictionary(Of String, Object)) As Boolean
        Dim sqlQuery As String = vbNullString

        sqlQuery = "UPDATE OBDR.dbo.TransDeposits" & vbCrLf &
                   "SET OBDR.dbo.TransDeposits.IsCancel = 1" & vbCrLf &
                   ", OBDR.dbo.TransDeposits.Remarks = " & ReadyString(Trim(inputValue("remarks"))) & vbCrLf &
                   "WHERE OBDR.dbo.TransDeposits.ID = " & ReadyString(inputValue("id"))
        If Common.WebRequest.ProData.ExecuteSQL(sqlQuery) = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetHistoricalDeposits(ByVal inputValue As Dictionary(Of String, Object)) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT OBDR.dbo.TransDeposits.ID" & vbCrLf &
                  ", OBDR.dbo.LibraryBranchBankInfo.AccountNumber" & vbCrLf &
                  ", OBDR.dbo.LibraryBranchBankInfo.AccountName + ' - ' + OBDR.dbo.TransDeposits.TransactionType AS [AccountName]" & vbCrLf &
                  ", DavaoUnifiedPayroll.dbo.LibraryBranch.ID AS [BranchID]" & vbCrLf &
                  ", DavaoUnifiedPayroll.dbo.LibraryBranch.Name" & vbCrLf &
                  ", OBDR.dbo.LibraryPOSDescriptions.ID AS [POSDescriptionID]" & vbCrLf &
                  ", OBDR.dbo.LibraryPOSDescriptions.Descriptions" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.DepositAmount" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.SalesDate" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.DateDeposits" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.RovingTeller" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.IsCheckPayment" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.CheckTotalAmount" & vbCrLf &
                  "FROM OBDR.dbo.TransDeposits INNER JOIN" & vbCrLf &
                  "OBDR.dbo.LibraryBranchBankInfo ON OBDR.dbo.LibraryBranchBankInfo.ID = OBDR.dbo.TransDeposits.BankInfoID INNER JOIN" & vbCrLf &
                  "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryBranchBankInfo.BranchID INNER JOIN" & vbCrLf &
                  "LibraryBanks ON LibraryBanks.ID = OBDR.dbo.LibraryBranchBankInfo.BankID INNER JOIN" & vbCrLf &
                  "OBDR.dbo.LibraryPOSDescriptions ON OBDR.dbo.LibraryPOSDescriptions.ID = OBDR.dbo.TransDeposits.POSDescriptionID" & vbCrLf &
                  "WHERE (Convert(DateTime, FLOOR(Convert(float, OBDR.dbo.TransDeposits.DateDeposits)))) BETWEEN " & ReadyString(inputValue("datestart")) & vbCrLf &
                  " AND " & ReadyString(inputValue("dateend")) & " AND OBDR.dbo.TransDeposits.IsCancel = 0 AND OBDR.dbo.LibraryBranchBankInfo.BankID = " & ReadyString(inputValue("bankid")) & vbCrLf &
                  " AND DavaoUnifiedPayroll.dbo.LibraryBranch.ID = " & inputValue("branchid") & vbCrLf &
                  " ORDER BY OBDR.dbo.TransDeposits.DateDeposits ASC"

        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function

    Public Function GetHistoricalBySalesDate(ByVal startDate As Date, ByVal endDate As Date) As DataTable
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable

        sqlQuery = "SELECT OBDR.dbo.TransDeposits.ID" & vbCrLf &
                  ", OBDR.dbo.LibraryBranchBankInfo.AccountNumber" & vbCrLf &
                  ", OBDR.dbo.LibraryBranchBankInfo.AccountName + ' - ' + OBDR.dbo.TransDeposits.TransactionType AS [AccountName]" & vbCrLf &
                  ", DavaoUnifiedPayroll.dbo.LibraryBranch.ID AS [BranchID]" & vbCrLf &
                  ", DavaoUnifiedPayroll.dbo.LibraryBranch.Name" & vbCrLf &
                  ", OBDR.dbo.LibraryPOSDescriptions.ID AS [POSDescriptionID]" & vbCrLf &
                  ", OBDR.dbo.LibraryPOSDescriptions.Descriptions" & vbCrLf &
                  ", OBDR.dbo.LibraryPOSDescriptions.MachineNumber" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.DepositAmount" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.SalesDate" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.DateDeposits" & vbCrLf &
                  ", OBDR.dbo.TransDeposits.RovingTeller" & vbCrLf &
                  "FROM OBDR.dbo.TransDeposits INNER JOIN" & vbCrLf &
                  "OBDR.dbo.LibraryBranchBankInfo ON OBDR.dbo.LibraryBranchBankInfo.ID = OBDR.dbo.TransDeposits.BankInfoID INNER JOIN" & vbCrLf &
                  "DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = OBDR.dbo.LibraryBranchBankInfo.BranchID INNER JOIN" & vbCrLf &
                  "LibraryBanks ON LibraryBanks.ID = OBDR.dbo.LibraryBranchBankInfo.BankID INNER JOIN" & vbCrLf &
                  "OBDR.dbo.LibraryPOSDescriptions ON OBDR.dbo.LibraryPOSDescriptions.ID = OBDR.dbo.TransDeposits.POSDescriptionID" & vbCrLf &
                  "WHERE (Convert(DateTime, FLOOR(Convert(float, OBDR.dbo.TransDeposits.DateDeposits)))) BETWEEN " & ReadyString(startDate) & vbCrLf &
                  " AND " & ReadyString(endDate) & " AND OBDR.dbo.TransDeposits.IsCancel = 0" & vbCrLf &
                  " AND OBDR.dbo.TransDeposits.TransactionType = 'POSSALES'" & vbCrLf &
                  " ORDER BY OBDR.dbo.TransDeposits.DateDeposits, DavaoUnifiedPayroll.dbo.LibraryBranch.ID, OBDR.dbo.LibraryBranchBankInfo.AccountName ASC"

        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        Return tableRecords
    End Function
#End Region
End Class

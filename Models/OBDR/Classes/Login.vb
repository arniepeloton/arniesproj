Public Class Login
    Public Function AccessClient(ByVal Username As String, ByVal Password As String) As Dictionary(Of String, String)
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable
        Dim indexCounter As Integer = 0
        Dim tmpPassword As String = vbNullString
        Dim tmpUsers As New Dictionary(Of String, String)

        CheckSecurityKey()
        sqlQuery = "SELECT OBDR.dbo.LibraryUserAccountLogin.ID" & vbCrLf &
                   " , OBDR.dbo.LibraryUserAccountLogin.Username" & vbCrLf &
                   " , OBDR.dbo.LibraryUserAccountLogin.Password" & vbCrLf &
                   " , OBDR.dbo.LibraryUserAccounts.BankID" & vbCrLf &
                   " , OBDR.dbo.LibraryUserAccounts.ID AS [AccountID]" & vbCrLf &
                   " FROM OBDR.dbo.LibraryUserAccountLogin INNER JOIN" & vbCrLf &
                   " OBDR.dbo.LibraryUserAccounts ON OBDR.dbo.LibraryUserAccounts.ID = OBDR.dbo.LibraryUserAccountLogin.UserAccountID" & vbCrLf &
                   "WHERE OBDR.dbo.LibraryUserAccountLogin.Username = " & ReadyString(Username)
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)
        If tableRecords.Rows.Count > 0 Then
            'verify password
            While indexCounter < tableRecords.Rows.Count
                tmpPassword = rsaKeys.decryptMessage(tableRecords.Rows(indexCounter)("Password"))
                If tmpPassword = Password Then
                    With tmpUsers
                        .Add("BankUserAccountID", tableRecords.Rows(indexCounter)("AccountID").ToString)
                        .Add("Username", tableRecords.Rows(indexCounter)("Username"))
                        .Add("BankID", tableRecords.Rows(indexCounter)("BankID").ToString)
                    End With
                    Exit While
                End If
                indexCounter += 1
            End While
        End If
        Return tmpUsers
    End Function
End Class

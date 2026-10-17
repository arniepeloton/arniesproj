Public Class Login
    Public Function AccessClient(ByVal Username As String, ByVal Password As String) As Dictionary(Of String, String)
        Dim sqlQuery As String = vbNullString
        Dim tableRecords As New DataTable
        Dim indexCounter As Integer = 0
        Dim tmpPassword As String = vbNullString
        Dim tmpUsers As New Dictionary(Of String, String)

        CheckSecurityKey()
        sqlQuery = "SELECT * FROM LibraryUserAccountLogin" & vbCrLf &
                   "WHERE Username = " & ReadyString(Username, False)
        tableRecords = mProData.Execute(sqlQuery)
        If tableRecords.Rows.Count > 0 Then
            'verify password
            While indexCounter < tableRecords.Rows.Count
                tmpPassword = rsaKeys.decryptMessage(tableRecords.Rows(indexCounter)("Password"))
                If tmpPassword = Password Then
                    With tmpUsers
                        .Add("Username", tableRecords.Rows(indexCounter)("Username"))
                        .Add("Password", tmpPassword)
                    End With
                    Exit While
                End If
                indexCounter += 1
            End While
        End If
        Return tmpUsers
    End Function
End Class

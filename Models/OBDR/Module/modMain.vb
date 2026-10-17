Module modMain

#Region "Global Variables"
    'Public rsaKeys As New utilities.RsaEncryptions
    Public jss As New JavaScriptSerializer
    Public BankUserAccess As New Login
    Public accountManagement As New AccountManagements
    Public posNumberManagement As New POSNumberManagements
    Public rsaKeys As New RsaEncryptions
#End Region

#Region "Global Function"

    Public Sub CheckSecurityKey()
        Dim tableRecords As New DataTable
        Dim sqlQuery As String = vbNullString

        sqlQuery = "SELECT * FROM OBDR.dbo.SystemKeys"
        tableRecords = Common.WebRequest.ProData.Execute(sqlQuery)

        If tableRecords.Rows.Count > 0 Then
            'Return Keys
            With rsaKeys
                .setGetPrivateKey = tableRecords.Rows(0)("PrivateKey")
                .setGetPublicKey = tableRecords.Rows(0)("PublicKey")
            End With
        Else
            'Create keys
            With rsaKeys
                .generateSecurityKeys()
                sqlQuery = "INSERT INTO OBDR.dbo.SystemKeys(OBDR.dbo.SystemKeys.PublicKey,OBDR.dbo.SystemKeys.PrivateKey)" & vbCrLf &
                           "VALUES(" & ReadyString(.setGetPublicKey) & ", " & ReadyString(.setGetPrivateKey) & ")"
                Common.WebRequest.ProData.ExecuteSQL(sqlQuery)
            End With
        End If
    End Sub
#End Region
End Module

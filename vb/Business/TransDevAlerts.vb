Public Class TransDevAlerts
    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Sub AddEntry(vMessage As String, vSource As String,
        Optional vOrgID As String = Nothing,
        Optional vDeviceID As String = Nothing,
        Optional vFowstID As String = Nothing,
        Optional vLoggedInUserID As String = Nothing)

        Dim pIU As InsertUpdate
        Dim pSql As String

        Try
            pIU = New InsertUpdate("MobizTransDevAlerts")
            pIU("DateLogged") = "GETDATE()"
            pIU("Message") = ReadyString(vMessage)
            pIU("Source") = ReadyString(vSource)
            If vOrgID IsNot Nothing Then
                pIU("OrgID") = ReadyString(vOrgID)
            End If
            If vDeviceID IsNot Nothing Then
                pIU("DeviceID") = ReadyString(vDeviceID)
            End If

            If vFowstID IsNot Nothing Then
                pIU("FowstID") = ReadyString(vFowstID)
            End If
            If vLoggedInUserID IsNot Nothing Then
                pIU("LoggedInUserID") = ReadyString(vLoggedInUserID)
            End If
            pSql = pIU.GetInsert
            mProData.ExecuteSQL(pSql)
        Catch ex As Exception
            AppendToLog("vbRT9XSZR7GJ! AddEntry: " & ex.Message)
            'Stop2()
        End Try

    End Sub

End Class

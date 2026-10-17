Public Class TransUnflushedCounts

    Dim mProData As ProData
    Dim mParent As Business

    Private Shared ReadOnly mExtractLogsLock As New Object

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Sub Save(vInput As InputClass)

        If vInput.TablesUnflushedCount?.Count() = 0 Then
            Return
        End If
        Dim pDrDevice = mParent.LibraryDevice.GetByUUID(vInput.DeviceSerial)
        If pDrDevice Is Nothing Then
            Throw New ApplicationException("Device with UUID, " & vInput.DeviceSerial & ", does not exist. vbKGZHGL9M5A!")
        End If
        Dim pDeviceID = CStr2(pDrDevice("ID"))
        For Each pItem In vInput.TablesUnflushedCount
            Dim pIU = New InsertUpdate("TransUnflushedCounts")
            Dim pNow = Now
            pIU("ID") = "NEWID()"
            pIU("FlushingDate") = ReadyString(pNow.ToString(cGlobal.FormatDateTimeSql2))
            pIU("FowstID") = ReadyString(vInput.FowstID)
            pIU("DeviceID") = ReadyString(pDeviceID)
            Dim pTransactionDate = ParseDate(pItem.Key)
            If pTransactionDate Is Nothing Then
                Continue For
            End If
            pIU("TransactionDate") = ReadyString(pTransactionDate.Value.ToString(cGlobal.FormatDateSql))
            pIU("UnflushedCount") = pItem.Value
            Dim pSql = "SELECT ID" & vbCrLf &
                " FROM TransUnflushedCounts" & vbCrLf &
                " WHERE CONVERT(Date, FlushingDate) = " & ReadyString(pNow.ToString(cGlobal.FormatDateSql)) & vbCrLf &
                " AND FowstID = " & pIU("FowstID") & vbCrLf &
                " AND DeviceID = " & pIU("DeviceID") & vbCrLf &
                " AND TransactionDate = " & pIU("TransactionDate")
            Dim pID = CStr2(mProData.ExecuteScalar(pSql))
            If pID = "" Then
                pSql = pIU.GetInsert()
            Else
                pSql = pIU.GetUpdate("ID = " & ReadyString(pID))
            End If
            mProData.ExecuteSQL(pSql)
        Next

    End Sub

    Public Class InputClass
        Public OrgID As String
        Public FowstID As String
        Public DeviceSerial As String
        Public TablesUnflushedCount As Dictionary(Of String, Integer)
    End Class
End Class
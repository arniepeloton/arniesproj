Public Class CacheSalesTablesStarter

    Dim mProData As ProData
    Dim mParent As Business

#If DEBUG Then
    Const mStartingDate As Date = #2025-10-01#
#Else
    Const mStartingDate As Date = #2026-01-01#
#End If

    Dim mLogFile As String

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Sub AddLog(vText As String)

        AppendToLog(vText, "temp\logs\SalesTable\Starter", mLogFile)

    End Sub

    Public Sub Execute(ByRef oError As String)

        mLogFile = DateTimeStamp()
        AddLog("Execute")
        Dim pSql = "SELECT ID" & vbCrLf &
            ",Name" & vbCrLf &
            ",Code" & vbCrLf &
            " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
            " WHERE LTRIM(RTRIM(ISNULL(Code, ''))) <> ''" & vbCrLf &
            " ORDER BY ID"
        Dim pDtFowsts = mProData.Execute(pSql)
        AddLog("vbUXW7NYEZU4! Fowst count: " & pDtFowsts.Rows.Count.ToString(cGlobal.FormatInteger))
        For Each pDrFowst As DataRow In pDtFowsts.Rows
            ExecuteFowst(pDrFowst)
        Next
        AddLog("vbF9UKQ58PRJ! All done.")

    End Sub

    Sub ExecuteFowst(vDrFowst As DataRow)

        Dim pSql = "SELECT MIN(CacheInvoices.InvoiceDate) AS InvoiceDate" & vbCrLf &
            " FROM TransInvoice" & vbCrLf &
            " JOIN CacheInvoices ON CacheInvoices.ID = TransInvoice.ID" & vbCrLf &
            " WHERE CacheInvoices.FowstID = " & ReadyString(vDrFowst("ID")) & vbCrLf &
            " AND TransInvoice.SpaviCacheDate IS NULL" & vbCrLf &
            " AND CacheInvoices.InvoiceNo > 0"
        Dim pDate = CDateNull(mProData.ExecuteScalar(pSql))
        If pDate Is Nothing Then
            Return
        End If

        pDate = pDate.Value.Date
        pSql &= vbCrLf &
            " AND CONVERT(date, CacheInvoices.InvoiceDate) > " & ReadyString(pDate.Value)
        Dim pDate2 = CDateNull(mProData.ExecuteScalar(pSql))
        If pDate2 Is Nothing Then
            Return
        End If

        AddLog("vbBZCED9GUTS! ExecuteFowst: " & pDate.Value.ToString(cGlobal.FormatDate) & " - " & ReadyGuid(vDrFowst("ID")) & " - " & CStr2(vDrFowst("Code")) & " - " & CStr2(vDrFowst("Name")))
        mParent.CacheSalesTables.InvoiceDate = pDate
        mParent.CacheSalesTables.FowstID = CStr2(vDrFowst("ID"))
        mParent.CacheSalesTables.Execute("dummy")
        AddLog("vbRJ6D8TAZKA! ExecuteFowst done.")

    End Sub
End Class
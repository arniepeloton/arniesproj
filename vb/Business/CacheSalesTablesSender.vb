Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports WebProj.CacheSalesTables

Public Class CacheSalesTablesSender

    Dim mProData As ProData
    Dim mParent As Business

#If DEBUG Then
    Const mStartingDate As Date = #2025-10-01#
    Const mEnableSending As Boolean = False
#Else
    Const mStartingDate As Date = #2026-01-01#
    Const mEnableSending As Boolean = True
#End If

    Dim mLogFile As String
    'Shared mHttpClient = New HttpClient()
    Dim mLastResponse As LogInput
    Dim mLastFailResponse As LogInput

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Sub AddLog(vText As String, Optional vException As Exception = Nothing)

        If vException IsNot Nothing Then
            vText = CStr2(vText).Trim() & vbCrLf & vbCrLf & GetStackTrace(, vException)
        End If
        AppendToLog(vText, "temp\logs\SalesTable\Sender", mLogFile)

    End Sub

    Public Async Function Execute() As Task

        Try
            Do
                mLogFile = DateTimeStamp()
                AddLog("Execute Start vb7EBQN6M9MB!")
                Dim pSql = "SELECT ID" & vbCrLf &
                    ",Name" & vbCrLf &
                    ",Code" & vbCrLf &
                    " FROM MobizLibraryOrganizationFOWST" & vbCrLf &
                    " WHERE LTRIM(RTRIM(ISNULL(Code, ''))) <> ''" & vbCrLf &
                    " ORDER BY ID"
                Dim pDtFowsts = mProData.Execute(pSql)
                AddLog("vbNLJVJQGG2M! Fowst count: " & pDtFowsts.Rows.Count.ToString(cGlobal.FormatInteger))
                Dim pSuccessCount As Integer = 0
                For Each pDrFowst As DataRow In pDtFowsts.Rows
                    If Await ExecuteFowst(pDrFowst) Then
                        pSuccessCount += 1
                    End If
                Next
                AddLog("vbDLZFNJ6KG4! All done.")
                If pSuccessCount = 0 Then
                    Exit Do
                End If
                Await Task.Delay(5000)
            Loop
        Catch ex As Exception
            AddLog("vb92WQZ2QHY4!", ex)
        End Try

    End Function

    Async Function ExecuteFowst(vDrFowst As DataRow) As Task(Of Boolean)

        Dim pSql = "SELECT TOP 1 store_code" & vbCrLf &
            ",tx_date" & vbCrLf &
            ",terminal" & vbCrLf &
            " FROM CacheSalesTableInvoice" & vbCrLf &
            " WHERE store_code = " & ReadyString(vDrFowst("Code")) & vbCrLf &
            " AND tx_date >= " & ReadyDateTime(mStartingDate.Date) & vbCrLf &
            " AND DateSent IS NULL" & vbCrLf &
            " ORDER BY tx_date"
        Dim pDR = mParent.ProData.ExecuteRow(pSql)
        If pDR IsNot Nothing Then
            Return Await SendAllTables(pDR)
        End If

    End Function

    Public Async Function SendAllTables(pDrFirstStore As DataRow) As Task(Of Boolean)

        Try
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 Or
                System.Net.SecurityProtocolType.Tls11 Or
                System.Net.SecurityProtocolType.Tls

            AddLog("store_code = " & CStr2(pDrFirstStore("store_code")) & ". tx_date = " & CDate(pDrFirstStore("tx_date")).ToString(cGlobal.FormatDateSql))
            ' temp commented during testing 2026-03-31
            'If CheckTotals(pDrFirstStore("store_code"), pDrFirstStore("tx_date")) = False Then
            '    UpdateDateSent("CacheSalesTableInvoice", pDrFirstStore("store_code"), pDrFirstStore("terminal"), pDrFirstStore("tx_date"), "DateError")
            '    Continue Do
            'End If

            Dim pTables As New Dictionary(Of String, String)
            pTables.Add("order", "CacheSalesTableOrder")
            pTables.Add("invoice", "CacheSalesTableInvoice")
            pTables.Add("payment", "CacheSalesTablePayments")
            pTables.Add("financial", "CacheSalesTableFinancial")

            Dim pFailed As Boolean
            For Each pTable In pTables
                Await SendLog(Nothing, eEventType.DataPreparationStarted, pDrFirstStore("store_code"), pTable.Key,
                    pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
                Await SendLog(Nothing, eEventType.DataPreparationCompleted, pDrFirstStore("store_code"), pTable.Key,
                    pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)

                pFailed = Not Await SendTable(pTable.Value, pTable.Key, pDrFirstStore("store_code"), pDrFirstStore("tx_date")).ConfigureAwait(False)
                If pFailed Then
                    Await SendLog(mLastFailResponse, eEventType.DataSubmitRequestSent, pDrFirstStore("store_code"), pTable.Key,
                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
                    Await SendLog(mLastFailResponse, eEventType.DataSubmitResponse, pDrFirstStore("store_code"), pTable.Key,
                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
                Else
                    UpdateDateSent(pTable.Value, pDrFirstStore("store_code"), pDrFirstStore("terminal"), pDrFirstStore("tx_date"))
                    Await SendLog(mLastResponse, eEventType.DataSubmitRequestSent, pDrFirstStore("store_code"), pTable.Key,
                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
                    Await SendLog(mLastResponse, eEventType.DataSubmitResponse, pDrFirstStore("store_code"), pTable.Key,
                        pDrFirstStore("terminal"), CDate(pDrFirstStore("tx_date"))).ConfigureAwait(False)
                End If
            Next

            AddLog("All done for store_code = " & CStr2(pDrFirstStore("store_code")) & ". tx_date = " & CDate(pDrFirstStore("tx_date")).ToString(cGlobal.FormatDateSql))
            Return Not pFailed
        Catch ex As Exception
            AddLog("vbFRW3L3SLGZ! " & ex.Message, ex)
        End Try

    End Function

    Async Function SendLog(vInput As LogInput, vEventType As eEventType, vStoreCode As String, vTable As String, vTerminal As String, vTransactionDate As Date) As Task

        'Exit Function ' temp exit

        Const pLogUrl As String = "https://m03xqpcs15.execute-api.ap-southeast-1.amazonaws.com/uat/potato-corner/mpos"
        Const pLogKey As String = "EBC9LPIhmz4jSGWZjbdl7a7oaqNOekp62J8sOG5W"

        Try
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 Or
                System.Net.SecurityProtocolType.Tls11 Or
                System.Net.SecurityProtocolType.Tls

            Dim pAttempts = 1
            Do
                'Try
                '    AddLog("SendLog attempt #" & pAttempts & ". vEventType = " & vEventType.ToString() & ". vStoreCode = " & vStoreCode)
                '    Dim pJsonPayload = ComposeLogPayload(vInput, vEventType, vStoreCode, vTable, vTerminal, vTransactionDate)
                '    Dim apiContent = New StringContent(pJsonPayload, Encoding.UTF8, "application/json")
                '    apiContent.Headers.Add("x-api-key", pLogKey)
                '    Dim pSuccess As Boolean = False
                '    If mEnableSending Then
                '        Dim apiResponse = Await mHttpClient.PostAsync(pLogUrl, apiContent).ConfigureAwait(False)
                '        Dim apiBody = Await apiResponse.Content.ReadAsStringAsync()
                '        AddLog("Response = " & apiBody)
                '        Dim pResponseJson = JsonConvert.DeserializeObject(Of LogInput)(apiBody)
                '        pSuccess = pResponseJson.status?.Trim()?.ToLower() = "success"
                '    Else
                '        pSuccess = True
                '    End If
                '    If pSuccess Then
                '        Return
                '    Else
                '        pAttempts += 1
                '        If pAttempts > 3 Then
                '            mLastFailResponse = mLastResponse
                '            Return
                '        End If
                '    End If
                'Catch ex As Exception
                '    pAttempts += 1
                '    If pAttempts > 3 Then
                '        Exit Do
                '    End If
                'End Try
            Loop
        Catch ex As Exception
            AddLog(ex.Message)
            AppendToLog(ex.Message)
        End Try

    End Function

    Sub UpdateDateSent(vTable As String, vStoreCode As String, vTerminal As String, vTransactionDate As Date, Optional vFieldName As String = "DateSent")

        Dim pIU = New InsertUpdate(vTable)
        pIU(vFieldName) = "GETDATE()"
        Dim pSql = pIU.GetUpdate("tx_date = " & ReadyDateTime(vTransactionDate) & vbCrLf &
            " AND store_code = " & ReadyString(vStoreCode) & vbCrLf &
            " AND terminal = " & ReadyString(vTerminal) & vbCrLf &
            " AND DateSent IS NULL")
        Dim pCount = mParent.ProData.ExecuteSQL(pSql)

    End Sub

    Public Async Function SendTable(vTable As String, vName As String, vStoreCode As String, vTransactionDate As Date) As Task(Of Boolean)

        Const pApiUrl As String = "https://oedf7csjxg.execute-api.ap-southeast-1.amazonaws.com/uat/potatocorner/mpos"
        Const pApiKey As String = "Z98xuBjozw8YWVuRsNC1n8C76smKTWe15mzLLJPy"

        Try
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 Or
                System.Net.SecurityProtocolType.Tls11 Or
                System.Net.SecurityProtocolType.Tls

            Dim pAttempts = 1
            Do
                Try
                    AddLog("SendTable attempt #" & pAttempts & ". vStoreCode = " & vStoreCode & ". vTransactionDate = " & vTransactionDate.ToString(cGlobal.FormatDateSql))
                    Dim pDT = GetTable(vTable, vStoreCode, vTransactionDate)
                    Dim pJsonPayload = SerializeTable(pDT, vName)
                    AddLog("pJsonPayload:" & vbCrLf & TrimToLength(pJsonPayload, 10000))
                    SaveJsonText(pJsonPayload, vName, vStoreCode, vTransactionDate)
                    'Return True ' temp exit

                    Dim payload As Byte() = Encoding.UTF8.GetBytes(pJsonPayload)
                    'Dim uploadContent As New ByteArrayContent(payload)

                    '' 1️⃣ Request presigned URL
                    'Dim apiContent = New StringContent(pJsonPayload, Encoding.UTF8, "application/json")
                    'apiContent.Headers.Add("x-api-key", pApiKey)
                    'Dim pSuccess As Boolean = False
                    'If mEnableSending Then
                    '    Dim apiResponse = Await mHttpClient.PostAsync(pApiUrl, apiContent).ConfigureAwait(False)
                    '    Dim apiBody = Await apiResponse.Content.ReadAsStringAsync()
                    '    AddLog(apiBody)
                    '    mLastResponse = JsonConvert.DeserializeObject(Of LogInput)(apiBody)
                    '    pSuccess = mLastResponse.status?.Trim()?.ToLower() = "success"
                    'Else
                    '    pSuccess = True
                    'End If
                    'If pSuccess Then
                    '    Return True
                    'Else
                    '    pAttempts += 1
                    '    If pAttempts > 3 Then
                    '        mLastFailResponse = mLastResponse
                    '        Return False
                    '    End If
                    'End If
                Catch ex As Exception
                    pAttempts += 1
                    If pAttempts > 3 Then
                        Exit Do
                    End If
                End Try
            Loop
        Catch ex As Exception
            AppendToLog(ex.Message)
        End Try

    End Function

    Function GetTable(vTable As String, vStoreCode As String, vTransactionDate As Date) As DataTable

        Dim pSql = "SELECT * FROM " & vTable &
            " WHERE CONVERT(date, tx_date) = " & ReadyDateTime(vTransactionDate.Date) & vbCrLf &
            " AND store_code = " & ReadyString(vStoreCode)
        AddLog("GetTable vbBCACHEDAUL! " & vbCrLf & pSql)
        Dim pDT = mParent.ProData.Execute(pSql)
        Return pDT

    End Function

    Sub SaveJsonText(vJson As String, vTableName As String, vStoreCode As String, vTransactionDate As Date)

        Dim pFilename = IO.Path.Combine(AppPath, "temp\logs\SalesTable\json", vTransactionDate.ToString("yyyy-MM-dd") & " " & vStoreCode & " " & vTableName & ".log")
        FileText(pFilename) = vJson

    End Sub

    Private Class SerializeTableInternalOutput
        Public table As String
        Public data As New List(Of Dictionary(Of String, Object))
    End Class

    Public Shared Function SerializeTable(vDT As DataTable, vTable As String) As String

        Dim pOut = New SerializeTableInternalOutput()
        pOut.table = vTable
        If vDT Is Nothing Then
            Return JsonConvert.SerializeObject(pOut, Formatting.Indented)
        End If

        ' format some columns
        For i As Integer = vDT.Columns.Count - 1 To 0 Step -1
            Dim pDC = vDT.Columns(i)
            If pDC.ColumnName.Trim().ToLower() = "tx_date" Then
                pDC.ColumnName = "tx_date_deletable"
                vDT.Columns.Add("tx_date")
                For Each pDR As DataRow In vDT.Rows
                    If pDR("tx_date_deletable") IsNot DBNull.Value Then
                        pDR("tx_date") = CDate(pDR("tx_date_deletable")).ToString("yyyy-MM-dd")
                    End If
                Next
                vDT.Columns.Remove(pDC)
            ElseIf pDC.ColumnName.Trim().ToLower() = "invoiceid" Then
                vDT.Columns.Remove(pDC)
            ElseIf pDC.DataType Is GetType(DateTime) Then
                Dim pColumnName = pDC.ColumnName
                pDC.ColumnName = pColumnName & "_deletable"
                vDT.Columns.Add(pColumnName)
                For Each pDR As DataRow In vDT.Rows
                    If pDR(pColumnName & "_deletable") IsNot DBNull.Value Then
                        pDR(pColumnName) = CDate(pDR(pColumnName & "_deletable")).ToString("yyyy-MM-ddTHH:mm:sszzz")
                    End If
                Next
                vDT.Columns.Remove(pDC)
            End If
        Next
        vDT.Columns.Remove("DateSent")

        pOut.data = JsonDictionary.ViewToObjectDictionary(vDT.DefaultView)

        Return JsonConvert.SerializeObject(pOut, Formatting.Indented)

    End Function

End Class
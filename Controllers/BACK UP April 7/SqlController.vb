Public Class SqlController
    Inherits System.Web.Mvc.Controller

    Dim mProData As ProData

    Public Function Index() As ActionResult

        If Request.HttpMethod = "POST" Then
            mProData = Common.WebRequest.ProData
            Run()
        Else
            If IO.File.Exists(Common.ServerPath & "settings\sql.txt") Then
                ViewData("SQL") = FileText(Common.ServerPath & "settings\sql.txt")
            End If
        End If
        Return View()

    End Function

    Private Sub Run()

        Dim pSqls() As String
        Dim pSql As String = ""
        Dim p, p2 As Integer
        Dim pStart, pEnd As Integer
        Dim pDS As DataSet
        Dim pTxt As String
        Dim pDelimiter As String '= gSqlDelimiter
        Dim pTimeStart As Date
        Dim pDT As DataTable

        Dim pDE As DataExport
        Dim pTableName As String = ""
        Dim pMS As IO.MemoryStream

        pTimeStart = Now
        pDS = New DataSet
        pTxt = Request("txtSql")
        ViewData("SQL") = pTxt
        FileText(Common.ServerPath & "settings\sql.txt") = pTxt

        If Regex.IsMatch(pTxt, "<\w+>") Then
            pDE = New DataExport
            pDE.ProData = Common.WebRequest.ProData
            pSqls = pTxt.Split(New String() {pDelimiter}, StringSplitOptions.RemoveEmptyEntries)
            For p = 0 To pSqls.Length - 1
                If pSqls(p).Trim.Length > 0 Then
                    pSql = pSqls(p)
                    If IsForDownload(pSql, pTableName) Then
                        pDE.AddTable(pTableName, pSql)
                    End If
                End If
            Next p

            pMS = pDE.ExportStream(Nothing)

            Response.Clear()
            Response.ContentType = "application/zip"
            Response.AddHeader("content-disposition", "attachment;filename=DataExport.zip") ' "inline" instead of "attachment"
            Response.BinaryWrite(pMS.ToArray)
            Response.End()
        End If

        Try
            pSqls = pTxt.Split(New String() {pDelimiter}, StringSplitOptions.RemoveEmptyEntries)
            For p = 0 To pSqls.Length - 1
                If pSqls(p).Trim.Length > 0 Then
                    pSql = pSqls(p)
                    pDT = mProData.Execute(pSql)
                    If pDT IsNot Nothing Then
                        pDT.TableName = "Table" & p
                        pDS.Tables.Add(pDT)
                    End If
                End If
            Next p
            ViewData("DataSet") = pDS

            ViewData("Message") = "Query executed successfully." & ControlChars.CrLf & _
                "Time elapsed: " & FormatTime(DateDiff(DateInterval.Second, pTimeStart, Now))
        Catch ex As Exception
            If Common.WebRequest.ProData.LastSqlException IsNot Nothing _
                AndAlso Common.WebRequest.ProData.LastSqlException.Rolledback = TriState.False Then

                Try
                    Common.WebRequest.ProData.RollbackTransaction(True)
                Catch ex2 As Exception

                End Try
            End If
            pTxt = pTxt.ToUpper
            If p > 0 Then
                For p2 = 0 To p - 1
                    pStart = pTxt.IndexOf(pDelimiter, pStart, StringComparison.OrdinalIgnoreCase) + pDelimiter.Length
                Next p2
            End If

            pEnd = pTxt.IndexOf(pDelimiter, pStart, StringComparison.OrdinalIgnoreCase)
            If pEnd = -1 Then
                pEnd = pTxt.Length - 1
            Else
                pEnd += 1
            End If

            ViewData("Message") = "Error in statement number " & Format(p + 1, cGlobal.FormatInteger) & _
                 ControlChars.CrLf & ex.Message & ControlChars.CrLf & "Statement: " & ControlChars.CrLf & pSql
        End Try

    End Sub

    Private Function IsForDownload(ByRef rSql As String, ByRef oTableName As String) As Boolean

        Dim pPos As Integer

        rSql = rSql.Trim
        If rSql.StartsWith("<") = False Then Return False
        pPos = rSql.IndexOf(">")
        If pPos = -1 Then Return False

        oTableName = rSql.Substring(1, pPos - 1)
        rSql = rSql.Substring(pPos + 1, rSql.Length - pPos - 1)

        Return True

    End Function
End Class

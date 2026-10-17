Public Class WebSqlController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As ActionResult
        Return PartialView("~/Views/Sql/Index.vbhtml")
        'Return View()

    End Function

    Private Function HasProjFile() As Boolean

        Dim pStr As String

        pStr = New Uri(Reflection.Assembly.GetExecutingAssembly.CodeBase).LocalPath ' D:\projects\Protonics\WebProj\bin\ProWeb.DLL
        pStr = IO.Path.GetDirectoryName(pStr) ' D:\projects\Protonics\WebProj\bin
        pStr = IO.Path.GetDirectoryName(pStr) ' D:\projects\Protonics\WebProj
        If IO.Directory.GetFiles(pStr, "*.vbproj").Length > 0 Then
            Return True
        End If
        If IO.Directory.GetFiles(pStr, "*.csproj").Length > 0 Then
            Return True
        End If
        If IO.Directory.GetFiles(pStr, "*.sln").Length > 0 Then
            Return True
        End If

    End Function

    <HttpPost>
    Public Function ExecuteSql() As String

        'If HasProjFile() Then
        '    Return JsonDictionary.Serialize("Error", "root directory has project file. vbK5QN42R52C!")
        'End If

        If Common.WebRequest.Data("password") <> "super!secret2" Then
            Return JsonDictionary.Serialize("Error", "Invalid password")
        End If

        Dim pSw = Stopwatch.StartNew()
        Dim pOut = New JsonDictionary()
        Try
            Dim pSql = Common.WebRequest.Data("sql")
            Common.WebRequest.ProData.CommandTimeout = CInt2(WebRequest.Instance.Data("timeout"))
            Dim pDT = Common.WebRequest.ProData.Execute(pSql)
            If pDT Is Nothing Then
                Dim pCount = Common.WebRequest.ProData.ExecuteScalar("SELECT @@ROWCOUNT")
                If IsNumeric(pCount) Then
                    pOut.Add("AffectedRecords", pCount)
                End If
            Else
                pOut.Add("Result", pDT)
            End If
        Catch ex As Exception
            pOut.Add("Error", ex.Message)
        End Try

        Dim pDuration = pSw.ElapsedMilliseconds
        pOut.Add("ElapsedMilliseconds", pDuration)
        Return pOut.Serialize()

    End Function
End Class

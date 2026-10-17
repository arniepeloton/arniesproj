Public Class DownloadController
    Inherits System.Web.Mvc.Controller

    ''' <summary>
    ''' allows the user to download files from restricted location
    ''' </summary>
    ''' <remarks></remarks>
    Function Index(ByVal vFilename As String) As ActionResult
        ' the vFilename parameter will work even if the url didnt specify a querystring for it because a route has been setup in global.asax

        Dim pFilename As String

        If CStr2(vFilename).Length = 0 Then Return Nothing
        pFilename = Common.ServerPath & "temp\documents\"
        DeleteOldFiles(pFilename)

        pFilename &= vFilename
        If IO.File.Exists(pFilename) Then
            Response.ContentType = "application/" & IO.Path.GetExtension(vFilename)
            Response.AddHeader("content-disposition", "attachment; filename=" & CreateValidFilename(vFilename, True))
            Response.TransmitFile(pFilename)
        End If

    End Function

    Private Sub DeleteOldFiles(ByVal vFolder As String)

        Dim pDT As DataTable

        Try
            If IO.Directory.Exists(vFolder) Then
                pDT = DirectoryToTable(vFolder, , , , , , Now.AddDays(-1))
                For Each pDR As DataRow In pDT.Rows
                    Try
                        IO.File.Delete(pDR("FullName"))
                    Catch ex As Exception
                        Stop2() ' just ignore errors
                    End Try
                Next pDR
            End If
        Catch ex As Exception
            Stop2() ' just ignore errors
        End Try

    End Sub

    Const mCompiledParent As String = "D:\PhoneGapMobiz\workspace\Mobiz\assets\www\"

    Public Function Compiled() As ActionResult

        Dim pFilename As String
        Dim pCd As Net.Mime.ContentDisposition

        pFilename = CombinePath(Request.Path)
        pCd = New Net.Mime.ContentDisposition
        pCd.FileName = IO.Path.GetFileName(pFilename)
        pCd.Inline = True
        'Response.AppendHeader("Content-Disposition", pCd.ToString())

        Return File(pFilename, "text/html")

    End Function

    Public Shared Function IsCompiledExist(vRelativeFilename As String) As Boolean

        Dim pFilename As String

        pFilename = CombinePath(vRelativeFilename)

        Return IO.File.Exists(pFilename)

    End Function

    Private Shared Function CombinePath(vRelativeFilename As String) As String

        If String.IsNullOrWhiteSpace(vRelativeFilename) Then
            Return mCompiledParent
        End If

        vRelativeFilename = vRelativeFilename.Replace("/", "\")
        If vRelativeFilename.StartsWith("\") Then
            vRelativeFilename = vRelativeFilename.Substring(1, vRelativeFilename.Length - 1)
        End If

        Return mCompiledParent & vRelativeFilename & ".html"

    End Function

End Class

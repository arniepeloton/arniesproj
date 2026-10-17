Namespace WebProj

    Public Class MobizAppController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function DownloadFile() As ActionResult
            Dim pFileTitle As String
            Dim pContentType As String
            Dim pSourceURL As String

            pFileTitle = "Mobiz.apk"
            pSourceURL = Common.ServerPath & "/Content/MobizAppsUpdate/" & pFileTitle
            pContentType = "application/apk"

            Return File(pSourceURL, pContentType, pFileTitle)

        End Function

    End Class

End Namespace
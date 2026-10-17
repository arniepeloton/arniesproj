Namespace WebProj
    Public Class VerifyEmailController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /VerifyEmail

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess() = False Then Exit Function
                If WebSession.Instance.IsEmailVerified Then Return RedirectToAction("Index", "SignIn")
            End If

            Return View()
        End Function

    End Class
End Namespace
Namespace WebProj
    Public Class SuccessController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Success

        Function Index() As ActionResult
            If CDec2(Request.QueryString("IsMobile")) = 0 Then
                WebSession.Instance.IsNew = 0
                WebSession.Instance.PeopleID = ""
                WebSession.Instance.PeopleUserID = ""

                ' If the user successfully registered (user is authorized to view this page)
                If User.Identity.IsAuthenticated Then

                    FormsAuthentication.SignOut() 'Clear the form
                    Return View()

                Else ' The user is not authorized. Therefore, redirect to home page
                    Return RedirectToAction("Index", "SignIn")
                End If
            Else
                Return View()
            End If
        End Function

    End Class
End Namespace
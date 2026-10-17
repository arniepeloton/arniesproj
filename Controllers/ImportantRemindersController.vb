Namespace WebProj
    Public Class ImportantRemindersController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /ImportantReminders

        Function Index() As ActionResult
            Return View()
        End Function


        Sub SaveIsAgreed()

            'Set cookie to authorize the user to see the sign up page
            FormsAuthentication.SetAuthCookie("User", False)

        End Sub
    End Class
End Namespace
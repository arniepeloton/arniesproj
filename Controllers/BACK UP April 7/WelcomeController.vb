Public Class WelcomeController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As ActionResult

        If CStr2(Common.WebSession.UserID).Length = 0 Then
            Transfer("/login")
        Else
            ViewData("FirstName") = WebSession.Instance.FirstName
            Return View()
        End If

    End Function

End Class
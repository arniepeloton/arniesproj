Public Class ProfileController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        If Common.WebSession.UserID.Length > 0 Then
            Return View()
        Else
            Transfer("/login")
        End If

    End Function

End Class

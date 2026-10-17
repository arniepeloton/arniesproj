Public Class ErrorController
    Inherits System.Web.Mvc.Controller

    Function Index(ByVal url As String) As ActionResult

        Response.StatusCode = 500
        ViewData("Url") = url

        Return View()

    End Function

    Public Function Http404(ByVal url As String) As String

        Return "Page not found</br>" & HttpUtility.HtmlEncode(url)

    End Function

    Public Function Denied(ByVal ReturnUrl As String)

        Return "Access denied</br>" & HttpUtility.HtmlEncode(ReturnUrl)

    End Function
End Class

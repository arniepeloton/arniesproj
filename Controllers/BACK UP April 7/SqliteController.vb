#If DEBUG Then
Public Class SqliteController
    Inherits System.Web.Mvc.Controller

    Public Function Index() As ActionResult
        Return View()
    End Function

    Public Function AutoIndexer() As ActionResult
        Return View()
    End Function
End Class
#End If

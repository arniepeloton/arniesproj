Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return View()

    End Function

#Region "Pages"
    Function AboutUs() As ActionResult

        Return View()

    End Function

    Function ContactDetails() As ActionResult

        Return View()

    End Function

    Function GiveUsFeedback() As ActionResult

        Return View()

    End Function
#End Region

    <HttpPost()>
    Public Function GetDate() As String

        Return JsonDictionary.Serialize("Value", Now)

    End Function
End Class

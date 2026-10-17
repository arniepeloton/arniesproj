Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        'Response.Redirect("/Signin/KB")
        'Response.Redirect("/Signin/CRM")
        If CBool2(FileText(Common.ServerPath & "settings\AllowEas.txt")) = False Then
            Response.Redirect("/Signin/" & Request.Url.Query)
        End If
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

        Dim pNow As DateTime
        Dim pValue As String

        pNow = Now
        pValue = Format(pNow, cGlobal.FormatDateTimeSql2)
        Return JsonDictionary.Serialize("Value", pValue)

    End Function

    Public Function DateTime() As String

        Return ReadyString(Now)

    End Function

    <HttpGet()>
    Public Function GetDateTime() As String

        Return ReadyString(Now)

    End Function

    Public Function GetClientIp() As String

        Dim pOut = Request.UserHostAddress

        Return pOut

    End Function
End Class

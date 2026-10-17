Public Class ServerAgentController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        App.RunAgent()

        Return View()

    End Function

    <HttpPost()>
    Public Function GetStatus() As String

        Dim pCharStart As Integer
        Dim pStr As String

        App.RunAgent()
        If Integer.TryParse(Common.WebRequest.Data("CharStart"), pCharStart) Then
            If CStr2(App.Message).Length > pCharStart Then
                pStr = App.Message.Substring(pCharStart)
            End If
        Else
            pStr = App.Message
        End If

        Return JsonDictionary.Serialize("Message", pStr)

    End Function
End Class

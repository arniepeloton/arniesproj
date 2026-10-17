Namespace WebProj
    Public Class CitizenshipController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function getCitizenshipList(ByVal Keywords As String) As String

            Dim clsCitizenship As New Citizenship

            Return DataTableToJSON.GetJson(clsCitizenship.getCitizenshipList(Keywords))

        End Function

    End Class
End Namespace
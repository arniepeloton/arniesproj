Namespace WebProj
    Public Class CentersController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function GetData() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT * FROM LibraryExpensesCenters WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function Save() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("LibraryExpensesCenters")

                p("Name") = ReadyString(Common.WebRequest.Data("Name"))
                p("Description") = ReadyString(Common.WebRequest.Data("Description"))
                p("IsActive") = ReadyString(Common.WebRequest.Data("IsActive"))
                p("ModifiedDate") = "getdate()"
                p("ModifiedBy") = ReadyString(WebSession.Instance.PeopleUserID)
                p("Type") = ReadyString(Common.WebRequest.Data("Type"))

                If Common.WebRequest.Data("ID") = "" Then
                    p("DateCreated") = "getdate()"
                    p("ID") = ReadyString(Guid.NewGuid.ToString())

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ID"))))
                End If

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryExpensesCenters")

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function
    End Class
End Namespace
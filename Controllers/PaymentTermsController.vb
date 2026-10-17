Public Class PaymentTermsController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function

    Function GetPaymentTerm() As String
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSql As String = "SELECT * FROM LibraryPaymentTerms WHERE ID = " & ReadyString(Common.WebRequest.Data("ID"))

        Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
    End Function

    Function SavePaymentTerms() As Object
        If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim ID As String = Common.WebRequest.Data("ID")
        Dim Name As String = Common.WebRequest.Data("Name")
        Dim Description As String = Common.WebRequest.Data("Description")
        Dim Days As String = Common.WebRequest.Data("Days")
        Dim IsActive As String = Common.WebRequest.Data("IsActive")
        Dim p As New InsertUpdate("LibraryPaymentTerms")
        Dim pSql As String

        Try
            p("Name") = ReadyString(Name)
            p("Description") = ReadyString(Description)
            p("Days") = ReadyString(Days)
            p("IsActive") = ReadyString(IsActive)
            p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "GetDate()"

            If ID = "" Then
                p("ID ") = ReadyString(Guid.NewGuid.ToString())
                pSql = p.GetInsert()
            Else
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

            Return Json(New With {Key .Results = "saved"})
        Catch ex As Exception
            Return Json(New With {Key .Results = "ERROR: " & ex.Message()})
        End Try
    End Function

End Class

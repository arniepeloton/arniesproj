Namespace WebProj
    Public Class SalaryGradeController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function GetSalaryGrade() As String
            Dim clsSalaryGrade As New SalaryGrade
            Dim strReturn As String

            'If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strReturn = JsonDictionary.Serialize("Results", clsSalaryGrade.GetSalaryGrade)

            Return strReturn
        End Function
    End Class
End Namespace
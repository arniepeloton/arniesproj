Public Class SalaryGrade
    Inherits System.Web.Mvc.Controller


    Function GetSalaryGrade() As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibrarySalaryGrade"
        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

End Class
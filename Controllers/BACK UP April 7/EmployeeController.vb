Namespace WebProj
    Public Class EmployeeController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function Menu() As ActionResult
            Return View()
        End Function

        Function EmployeeInfo() As ActionResult
            Return View()
        End Function

        Function getEmployeeInfo(ByVal ID As String) As String
            Dim clsEmployee As New Employee

            Return DataTableToJSON.GetJson(clsEmployee.getEmployeeInfo(ID))

        End Function

        Function getEmployeeList(ByVal KeyWords As String) As String
            Dim clsEmployee As New Employee

            Return DataTableToJSON.GetJson(clsEmployee.getEmployeeList(KeyWords))

        End Function

        Function getCityList(ByVal Keywords As String) As String

            Dim clsCity As New City

            Return DataTableToJSON.GetJson(clsCity.getCityList(Keywords))

        End Function

    End Class
End Namespace
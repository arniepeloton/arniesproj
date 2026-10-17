Public Class ProWebGridController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /ProWebGrid

    Function Index() As ActionResult

        ViewData("Data") = GetData()
        Return View()

    End Function

    Public Function GetData() As String

        Dim pDT As DataTable
        Dim pSql As String

        'Response.ContentType = "application/json"

        pSql = "SELECT * FROM LibraryDepartments"
        pDT = Common.WebRequest.ProData.Execute(pSql)

        Return JsonDictionary.Serialize(pDT)

    End Function

End Class

Public Class CustomerSearchController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function

    <Obsolete()>
    <HttpPost()>
    Public Function Search() As String

        Dim pFinder As Finder
        Dim pDT As DataTable

        HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*")

        pFinder = New Finder
        With pFinder
            .MaxRecords = 50
            .SearchString = Common.WebRequest.Data("keywords")
            .ProData = Common.WebRequest.ProData
            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Searchable = False
                End With
                With .Add("CompanyName", SqlDbType.VarChar)
                    '.Expression = "CompanyName"
                End With
                .Add("ContactNumber", SqlDbType.VarChar)
                .Add("TinNumber", SqlDbType.VarChar)
            End With
            .SqlFrom = "FROM LibraryCustomers"
            .SqlOrder = "ORDER BY CompanyName"
            .SqlWhere = "WHERE IsActive = 1"
            pDT = .Requery()
        End With

        Return JsonDictionary.Serialize("Results", pDT)

    End Function
End Class

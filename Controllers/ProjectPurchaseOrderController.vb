Namespace WebProj
    Public Class ProjectPurchaseOrderController
        Inherits System.Web.Mvc.Controller

        Dim clsProjectPO As New ProjectPurchaseOrder

        Function Index() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function GetPO() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsProjectPO.FilterBy = Common.WebRequest.Data("filterBy")
            clsProjectPO.Searchable = Common.WebRequest.Data("Searchable")
            clsProjectPO.SearchedText = Common.WebRequest.Data("SearchedText")

            Return JsonDictionary.Serialize("Results", clsProjectPO.GetPO())
        End Function
    End Class
End Namespace


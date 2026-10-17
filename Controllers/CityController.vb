Namespace WebProj
    Public Class CityController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function SearchCity() As ActionResult
            Return View()
        End Function

        Function getCityList(ByVal Keywords As String) As String

            Dim clsCity As New City

            Return DataTableToJSON.GetJson(clsCity.getCityList(Keywords))

        End Function

        Function getCityListAll() As String

            Dim clsCity As New City

            Return JsonDictionary.Serialize("CityData", clsCity.getCityListAll())

        End Function
    End Class
End Namespace
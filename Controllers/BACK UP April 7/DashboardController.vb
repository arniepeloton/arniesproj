Namespace WebProj
    Public Class DashboardController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /Dashboard

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                ' If (Not SecurityRole.Instance.HandleRoleAccess("Point of Sales (POS)")) Then Exit Function
            End If

            Return View()

        End Function

        'Function SignIn() As ActionResult
        '    If CStr2(Common.WebRequest.QueryString("mode")).ToLower = "logout" Then
        '        Common.WebSession.UserID = ""
        '    End If

        '    FormsAuthentication.SignOut()

        '    Response.Cache.SetCacheability(HttpCacheability.NoCache)
        '    Response.Cache.SetNoStore()

        '    Return View()

        'End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then
                pOut.Add("PeopleID", "")
            Else

                pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
                pOut.Add("PeopleID", WebSession.Instance.PeopleID)

            End If

            Return pOut.Serialize

        End Function
        'Update Manager
        <HttpPost()>
        Function SaveUpdateInfo() As String
            Dim clsUpdate As New Dashboard
            Dim strReturn As String

            clsUpdate.Product = Common.WebRequest.Data("Product")
            clsUpdate.Category = Common.WebRequest.Data("Category")
            clsUpdate.Title = Common.WebRequest.Data("Title")
            clsUpdate.Message = Common.WebRequest.Data("Message")
            clsUpdate.PeopleID = WebSession.Instance.PeopleUserID
           
            strReturn = JsonDictionary.Serialize("UpdateData", clsUpdate.SaveUpdateInfo())

            Return strReturn

        End Function
        'Registration


        <HttpPost()>
        Function getCityNameList() As String
            Dim strReturn As String
            Dim dataTblCityList As DataTable

            dataTblCityList = Common.WebRequest.ProData.Execute("SELECT ID, Name FROM LibraryCities ORDER BY Name")


            strReturn = JsonDictionary.Serialize("CityData", dataTblCityList)

            Return strReturn

        End Function

        <HttpPost()>
        Function getPeopleName() As String
            Dim data As New Dashboard

            If (Not WebSession.Instance.PeopleUserID Is Nothing) Then
                data.PeopleID = WebSession.Instance.PeopleUserID

                Return JsonDictionary.Serialize("PeopleName", data.GetPeopleName())
            Else
                Return JsonDictionary.Serialize("PeopleName", "")
            End If

        End Function

        'Registration
        <HttpPost()>
        Function getListOfBusiness() As String
            Dim s As New Dashboard
            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("DashboardList", s.GetListOfBusiness())
        End Function

        <HttpPost()>
        Function getDetails() As String
            Dim s As New Dashboard
            'If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Details", s.GetDetails())
        End Function

        Function getSearchResult() As String

            Dim clsRecord As New Dashboard

            'Return DataTableToJSON.GetJson(clsRecord.GetSearchResult(Keywords))
            Return JsonDictionary.Serialize("DashboardSearchResults", clsRecord.GetSearchResult())
        End Function
    End Class
End Namespace
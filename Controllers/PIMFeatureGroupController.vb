Namespace WebProj
    Public Class PIMFeatureGroupController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMFeatureGroupInfo() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.PIMFeatureGroupID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMFeatureGroupID", WebSession.Instance.PIMFeatureGroupID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckUniqueFeatureGroup() As String
            Dim clsPIMFeatureGroup As New PIMFeatureGroup
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMFeatureGroup.ID = Common.WebRequest.Data("FeatureGroupID")
            clsPIMFeatureGroup.FeatureGroupLabelID = Common.WebRequest.Data("FeatureGroupLabelID")

            strReturn = JsonDictionary.Serialize("FeatureGroupData", clsPIMFeatureGroup.CheckUniqueFeatureGroup())

            Return strReturn

        End Function

        <HttpPost()>
        Function getFeatureGroupList() As String
            Dim strKeyWord As String
            Dim clsPIMFeatureGroup As New PIMFeatureGroup
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("FeatureGroupData", clsPIMFeatureGroup.getFeatureGroupList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getFeatureGroupInfo() As String
            Dim strID As String
            Dim clsPIMFeatureGroup As New PIMFeatureGroup
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("FeatureGroupID")
            strReturn = JsonDictionary.Serialize("FeatureGroupData", clsPIMFeatureGroup.getFeatureGroupInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveFeatureGroupInfo() As String
            Dim clsPIMFeatureGroup As New PIMFeatureGroup
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMFeatureGroup.ID = Common.WebRequest.Data("FeatureGroupID")
            clsPIMFeatureGroup.FeatureGroupLabelID = Common.WebRequest.Data("FeatureGroupLabelID")

            WebSession.Instance.PIMFeatureGroupID = clsPIMFeatureGroup.SaveFeatureGroupInfo()
            strReturn = JsonDictionary.Serialize("FeatureGroupData", WebSession.Instance.PIMFeatureGroupID)

            Return strReturn

        End Function




    End Class

End Namespace
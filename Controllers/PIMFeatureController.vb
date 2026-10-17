Namespace WebProj
    Public Class PIMFeatureController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMFeatureInfo() As ActionResult
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
            WebSession.Instance.PIMFeatureID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMFeatureID", WebSession.Instance.PIMFeatureID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckUniqueFeature() As String
            Dim clsPIMFeature As New PIMFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMFeature.ID = Common.WebRequest.Data("PIMFeatureID")
            clsPIMFeature.FeatureLabelID = Common.WebRequest.Data("FeatureLabelID")
            clsPIMFeature.FeatureNameID = Common.WebRequest.Data("FeatureNameID")
            clsPIMFeature.FeatureMeasureID = Common.WebRequest.Data("FeatureMeasureID")
            clsPIMFeature.FeatureType = Common.WebRequest.Data("FeatureType")

            strReturn = JsonDictionary.Serialize("PIMFeatureData", clsPIMFeature.CheckUniqueFeature())

            Return strReturn

        End Function

        <HttpPost()>
        Function getFeatureList() As String
            Dim strKeyWord As String
            Dim clsPIMFeature As New PIMFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("PIMFeatureData", clsPIMFeature.getFeatureList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getFeatureOptionList() As String
            Dim strID As String
            Dim clsPIMFeature As New PIMFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = IIf(Common.WebRequest.Data("PIMFeatureID") Is Nothing, WebSession.Instance.PIMFeatureID, Common.WebRequest.Data("PIMFeatureID"))
            strReturn = JsonDictionary.Serialize("PIMFeatureData", clsPIMFeature.getFeatureOptionList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getFeatureInfo() As String
            Dim strID As String
            Dim clsPIMFeature As New PIMFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMFeatureID")
            strReturn = JsonDictionary.Serialize("PIMFeatureData", clsPIMFeature.getFeatureInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveFeatureInfo() As String
            Dim clsPIMFeature As New PIMFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMFeature.ID = Common.WebRequest.Data("PIMFeatureID")
            clsPIMFeature.FeatureLabelID = Common.WebRequest.Data("FeatureLabelID")
            clsPIMFeature.FeatureNameID = Common.WebRequest.Data("FeatureNameID")
            clsPIMFeature.FeatureMeasureID = Common.WebRequest.Data("FeatureMeasureID")
            clsPIMFeature.FeatureType = Common.WebRequest.Data("FeatureType")
            clsPIMFeature.DisplayFormat = Common.WebRequest.Data("DisplayFormat")

            clsPIMFeature.FeatureOptionList = Common.WebRequest.AllData("FeatureOptionList")

            WebSession.Instance.PIMFeatureID = clsPIMFeature.SavePIMFeatureInfo()
            strReturn = JsonDictionary.Serialize("PIMFeatureData", WebSession.Instance.PIMFeatureID)

            Return strReturn

        End Function




    End Class

End Namespace
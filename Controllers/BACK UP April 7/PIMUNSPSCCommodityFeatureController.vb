Namespace WebProj
    Public Class PIMUNSPSCCommodityFeatureController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMUNSPSCCommodityFeatureInfo() As ActionResult

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
            WebSession.Instance.PIMUNSPSCCommodityID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMUNSPSCCommodityID", WebSession.Instance.PIMUNSPSCCommodityID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function getUNSPSCCommodityList() As String
            Dim strKeyWord As String
            Dim clsPIMUNSPSCCommFeature As New PIMUNSPSCCommodityFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("PIMUNSPSCCommFeatureData", clsPIMUNSPSCCommFeature.getUNSPSCCommodityList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getUNSPSCCommFeatureList() As String
            Dim strID As String
            Dim clsPIMUNSPSCCommFeature As New PIMUNSPSCCommodityFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = IIf(Common.WebRequest.Data("PIMUNSPSCCommodityID") Is Nothing, WebSession.Instance.PIMUNSPSCCommodityID, Common.WebRequest.Data("PIMUNSPSCCommodityID"))
            strReturn = JsonDictionary.Serialize("PIMUNSPSCCommFeatureData", clsPIMUNSPSCCommFeature.getUNSPSCCommFeatureList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getUNSPSCCommodityInfo() As String
            Dim strID As String
            Dim clsPIMUNSPSCCommFeature As New PIMUNSPSCCommodityFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMUNSPSCCommodityID")
            strReturn = JsonDictionary.Serialize("PIMUNSPSCCommFeatureData", clsPIMUNSPSCCommFeature.getUNSPSCCommodityInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveUNSPSCommodityInfo() As String
            Dim clsPIMUNSPSCCommFeature As New PIMUNSPSCCommodityFeature
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMUNSPSCCommFeature.ID = Common.WebRequest.Data("PIMUNSPSCCommodityID")
            clsPIMUNSPSCCommFeature.UNSPSCCommFeatureList = Common.WebRequest.AllData("UNSPSCCommFeatureList")

            WebSession.Instance.PIMUNSPSCCommodityID = clsPIMUNSPSCCommFeature.SaveUNSPSCCommodityInfo()
            strReturn = JsonDictionary.Serialize("PIMUNSPSCCommFeatureData", WebSession.Instance.PIMUNSPSCCommodityID)

            Return strReturn

        End Function




    End Class

End Namespace
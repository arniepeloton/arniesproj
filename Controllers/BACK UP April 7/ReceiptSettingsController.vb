Namespace WebProj

    Public Class ReceiptSettingsController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("POS Receipt Configuration")) Then Exit Function
            End If

            Return View()
        End Function

        Function ReceiptSettingsInfo() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function ReceiptSettingsMessage() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()
        End Function

        Function ReceiptSettingsSummary() As ActionResult
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
            WebSession.Instance.ReceiptSettingsID = Common.WebRequest.Data("ID").ToString()

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("ReceiptSettingsID", WebSession.Instance.ReceiptSettingsID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function setSubscriberOrganization() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            WebSession.Instance.OrganizationID = WebRequest.Instance.OrgID
            pOut.Add("OrganizationID", WebSession.Instance.OrganizationID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function getReceiptSettingsList() As String
            Dim strKeyWord As String
            Dim clsReceiptSettings As New ReceiptSettings
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsReceiptSettings.OrganizationID = WebRequest.Instance.OrgID
            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("ReceiptSettingsData", clsReceiptSettings.getReceiptSettingsList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getMessageList() As String
            Dim strID As String
            Dim clsReceiptSettings As New ReceiptSettings
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.ReceiptSettingsID
            strReturn = JsonDictionary.Serialize("ReceiptSettingsData", clsReceiptSettings.getMessageList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPlacesList() As String
            Dim strID As String
            Dim clsReceiptSettings As New ReceiptSettings
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.ReceiptSettingsID
            strReturn = JsonDictionary.Serialize("ReceiptSettingsData", clsReceiptSettings.getPlacesList(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getReceiptSettingsInfo() As String
            Dim strID As String
            Dim clsReceiptSettings As New ReceiptSettings
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = WebSession.Instance.ReceiptSettingsID
            clsReceiptSettings.OrganizationID = WebRequest.Instance.OrgID
            strReturn = JsonDictionary.Serialize("ReceiptSettingsData", clsReceiptSettings.getReceiptSettingsInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveReceiptInfoName() As String
            Dim clsReceiptSettings As New ReceiptSettings
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsReceiptSettings.ID = Common.WebRequest.Data("ID")
            clsReceiptSettings.OrganizationID = WebRequest.Instance.OrgID
            clsReceiptSettings.OrganizationTradeNameID = Common.WebRequest.Data("OrganizationTradeNameID")
            clsReceiptSettings.PaperSize = Common.WebRequest.Data("PaperSize")
            clsReceiptSettings.MessageList = Common.WebRequest.AllData("MessageList")
            clsReceiptSettings.PlacesList = Common.WebRequest.AllData("PlacesList")

            WebSession.Instance.ReceiptSettingsID = clsReceiptSettings.SaveReceiptSettings()
            strReturn = JsonDictionary.Serialize("ReceiptSettingsData", WebSession.Instance.ReceiptSettingsID)

            Return strReturn

        End Function

        <HttpPost()>
        Function UploadReceiptLogo(ByVal objFile As HttpPostedFileBase) As String
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            objFile.SaveAs("../Content/images/" & WebSession.Instance.ReceiptSettingsID & ".png")

            strReturn = JsonDictionary.Serialize("ReceiptSettingsData", WebSession.Instance.ReceiptSettingsID)

            Return strReturn

        End Function


    End Class

End Namespace
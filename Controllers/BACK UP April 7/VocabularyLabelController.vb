Namespace WebProj
    Public Class VocabularyLabelController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function VocabularyLabelInfo() As ActionResult

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
            WebSession.Instance.VocabularyLabelID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("VocabularyLabelID", WebSession.Instance.VocabularyLabelID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckLanguageUniqueLabel() As String
            Dim clsVocabularyLabel As New VocabularyLabel
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsVocabularyLabel.ID = Common.WebRequest.Data("VocabularyLabelID")
            clsVocabularyLabel.Label = Common.WebRequest.Data("Label")

            strReturn = JsonDictionary.Serialize("VocabularyLabelData", clsVocabularyLabel.CheckLanguageUniqueLabel())

            Return strReturn

        End Function

        <HttpPost()>
        Function getVocabularyLabelList() As String
            Dim strKeyWord As String
            Dim clsVocabularyLabel As New VocabularyLabel
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("VocabularyLabelData", clsVocabularyLabel.getVocabularyLabelList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getVocabularyLabelInfo() As String
            Dim strID As String
            Dim clsVocabularyLabel As New VocabularyLabel
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("VocabularyLabelID")
            strReturn = JsonDictionary.Serialize("VocabularyLabelData", clsVocabularyLabel.getVocabularyLabelInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveVocabularyLabelInfo() As String
            Dim clsVocabularyLabel As New VocabularyLabel
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsVocabularyLabel.ID = Common.WebRequest.Data("VocabularyLabelID")
            clsVocabularyLabel.LanguageID = Common.WebRequest.Data("LanguageID")
            clsVocabularyLabel.Label = Common.WebRequest.Data("Label")

            WebSession.Instance.VocabularyLabelID = clsVocabularyLabel.SaveVocabularyLabelInfo()
            strReturn = JsonDictionary.Serialize("VocabularyLabelData", WebSession.Instance.VocabularyLabelID)

            Return strReturn

        End Function




    End Class

End Namespace
Namespace WebProj
    Public Class PIMMeasureController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMMeasureInfo() As ActionResult

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
            WebSession.Instance.PIMMeasureID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMMeasureID", WebSession.Instance.PIMMeasureID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckLanguageUniqueMeasure() As String
            Dim clsPIMMeasure As New PIMMeasure
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMMeasure.ID = Common.WebRequest.Data("PIMMeasureID")
            clsPIMMeasure.MeasureNameID = Common.WebRequest.Data("MeasureNameID")
            clsPIMMeasure.MeasureSignID = Common.WebRequest.Data("MeasureSignID")

            strReturn = JsonDictionary.Serialize("PIMMeasureData", clsPIMMeasure.CheckLanguageUniqueMeasure())

            Return strReturn

        End Function

        <HttpPost()>
        Function getMeasureList() As String
            Dim strKeyWord As String
            Dim clsPIMMeasure As New PIMMeasure
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("PIMMeasureData", clsPIMMeasure.getMeasureList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getMeasureInfo() As String
            Dim strID As String
            Dim clsPIMMeasure As New PIMMeasure
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMMeasureID")
            strReturn = JsonDictionary.Serialize("PIMMeasureData", clsPIMMeasure.getMeasureInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveMeasureInfo() As String
            Dim clsPIMMeasure As New PIMMeasure
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMMeasure.ID = Common.WebRequest.Data("PIMMeasureID")
            clsPIMMeasure.MeasureNameID = Common.WebRequest.Data("MeasureNameID")
            clsPIMMeasure.MeasureSignID = Common.WebRequest.Data("MeasureSignID")

            WebSession.Instance.PIMMeasureID = clsPIMMeasure.SaveMeasureInfo()
            strReturn = JsonDictionary.Serialize("PIMMeasureData", WebSession.Instance.PIMMeasureID)

            Return strReturn

        End Function




    End Class

End Namespace
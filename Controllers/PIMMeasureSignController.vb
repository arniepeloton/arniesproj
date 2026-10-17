Namespace WebProj
    Public Class PIMMeasureSignController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMMeasureSignInfo() As ActionResult

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
            WebSession.Instance.PIMMeasureSignID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("PIMMeasureSignID", WebSession.Instance.PIMMeasureSignID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckUniquePIMMeasureSign() As String
            Dim clsPIMMeasureSign As New PIMMeasureSign
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMMeasureSign.ID = Common.WebRequest.Data("PIMMeasureSignID")
            clsPIMMeasureSign.Sign = Common.WebRequest.Data("Sign")

            strReturn = JsonDictionary.Serialize("MeasureSignData", clsPIMMeasureSign.CheckUniquePIMMeasureSign())

            Return strReturn

        End Function

        <HttpPost()>
        Function getPIMMeasureSignList() As String
            Dim strKeyWord As String
            Dim clsPIMMeasureSign As New PIMMeasureSign
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("MeasureSignData", clsPIMMeasureSign.getPIMMeasureSignList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getPIMMeasureSignInfo() As String
            Dim strID As String
            Dim clsPIMMeasureSign As New PIMMeasureSign
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("PIMMeasureSignID")
            strReturn = JsonDictionary.Serialize("MeasureSignData", clsPIMMeasureSign.getPIMMeasureSignInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SavePIMMeasureSignInfo() As String
            Dim clsPIMMeasureSign As New PIMMeasureSign
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMMeasureSign.ID = Common.WebRequest.Data("PIMMeasureSignID")
            clsPIMMeasureSign.Sign = Common.WebRequest.Data("Sign")

            WebSession.Instance.PIMMeasureSignID = clsPIMMeasureSign.SavePIMMeasureSignInfo()
            strReturn = JsonDictionary.Serialize("MeasureSignData", WebSession.Instance.PIMMeasureSignID)

            Return strReturn

        End Function




    End Class

End Namespace
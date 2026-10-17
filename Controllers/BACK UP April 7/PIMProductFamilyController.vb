Namespace WebProj
    Public Class PIMProductFamilyController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
            End If

            Return View()

        End Function

        Function PIMProductFamilyInfo() As ActionResult

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
            WebSession.Instance.PIMProductFamilyID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("ProductFamilyID", WebSession.Instance.PIMProductFamilyID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckUniqueProductFamily() As String
            Dim clsPIMProductFamily As New PIMProductFamily
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProductFamily.ID = Common.WebRequest.Data("ProductFamilyID")
            clsPIMProductFamily.ProductFamilyLabelID = Common.WebRequest.Data("ProductFamilyLabelID")

            strReturn = JsonDictionary.Serialize("ProductFamilyData", clsPIMProductFamily.CheckUniqueProductFamily())

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductFamilyList() As String
            Dim strKeyWord As String
            Dim clsPIMProductFamily As New PIMProductFamily
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("ProductFamilyData", clsPIMProductFamily.getProductFamilyList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getProductFamilyInfo() As String
            Dim strID As String
            Dim clsPIMProductFamily As New PIMProductFamily
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = Common.WebRequest.Data("ProductFamilyID")
            strReturn = JsonDictionary.Serialize("ProductFamilyData", clsPIMProductFamily.getProductFamilyInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveProductFamilyInfo() As String
            Dim clsPIMProductFamily As New PIMProductFamily
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsPIMProductFamily.ID = Common.WebRequest.Data("ProductFamilyID")
            clsPIMProductFamily.ProductFamilyLabelID = Common.WebRequest.Data("ProductFamilyLabelID")

            WebSession.Instance.PIMProductFamilyID = clsPIMProductFamily.SaveProductFamilyInfo()
            strReturn = JsonDictionary.Serialize("ProductFamilyData", WebSession.Instance.PIMProductFamilyID)

            Return strReturn

        End Function




    End Class

End Namespace
Namespace WebProj
    Public Class DiscountSettingsController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /DiscountSettings

        Function Index() As ActionResult
            If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
                If WebRequest.Instance.HandleAccess = False Then Exit Function
                If (Not SecurityRole.Instance.HandleRoleAccess("Discount Settings")) Then Exit Function
            End If

            Return View()
        End Function

        'CREATED BY Janine Aug 24, 2013 1pm
        <HttpPost()>
        Function SaveDiscountSettings() As String
            Dim clsDiscount As New Discount
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            clsDiscount.SeniorCitizenDiscount = Common.WebRequest.Data("SeniorCitizenDiscount")
            clsDiscount.PersonWithDisabilityDiscount = Common.WebRequest.Data("PersonWithDisabilityDiscount")
            clsDiscount.EmployeeDiscount = Common.WebRequest.Data("EmployeeDiscount")
            clsDiscount.RegularDiscount1 = Common.WebRequest.Data("RegularDiscount1")
            clsDiscount.RegularDiscount2 = Common.WebRequest.Data("RegularDiscount2")
            clsDiscount.RegularDiscount3 = Common.WebRequest.Data("RegularDiscount3")
            clsDiscount.VIPDiscount = Common.WebRequest.Data("VIPDiscount")
            clsDiscount.GPCDiscount = Common.WebRequest.Data("GPCDiscount")
            clsDiscount.UserID = WebSession.Instance.PeopleUserID

            strReturn = JsonDictionary.Serialize("DiscountSettingsData", clsDiscount.SaveDiscountSettings())

            Return strReturn

        End Function

        <HttpPost()>
        Function getDiscountSettingsInfo() As String
            Dim strID As String
            Dim clsDiscount As New Discount
            Dim strReturn As String

            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            strID = IIf(Common.WebRequest.Data("DiscountID") Is Nothing, "", Common.WebRequest.Data("DiscountID"))
            WebSession.Instance.DeviceID = strID
            strReturn = JsonDictionary.Serialize("DiscountData", clsDiscount.getDiscountSettingsInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getDiscountSettings() As String
            Dim clsDiscount As New Discount
            Dim strReturn As String

            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            strReturn = JsonDictionary.Serialize("DiscountData", clsDiscount.getDiscountSettings())

            Return strReturn
        End Function

    End Class
End Namespace
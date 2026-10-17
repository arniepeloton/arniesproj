Namespace WebProj
    Public Class DeviceApprovalController
        Inherits System.Web.Mvc.Controller

        '
        ' GET: /DeviceApproval

        Function Index() As ActionResult

            'If WebSession.Instance.UserAccess Is Nothing _
            '            OrElse WebSession.Instance.UserAccess.Sales_Sales.IsEnable = False Then

            '    Transfer("/login")
            'Else
            '  If WebSession.Instance.UserAccess. IsEnable Then
            'Return View()
            ' Else
            '   Transfer("/AccessError")
            ' End If
            ' End If
            Return View()
        End Function





        <HttpPost()>
        Function getDeviceByID() As String

            Dim clsDeviceApproval As New DeviceApproval
            Dim strReturn As String

            '  If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            clsDeviceApproval.ID = WebRequest.Instance.Data("ID")


            strReturn = JsonDictionary.Serialize("DeviceList", clsDeviceApproval.getDeviceByDevID())
            Return strReturn
        End Function







        <HttpPost()>
        Function approveDeviceRegistrationRequestByID()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Dim clsDeviceApproval As New DeviceApproval
            Dim strReturn As String

            ' If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            '  If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            clsDeviceApproval.ID = WebRequest.Instance.Data("ID")
            clsDeviceApproval.updateDeviceByID()
        End Function


    End Class
End Namespace
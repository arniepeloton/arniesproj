Namespace WebProj
    Public Class DevicePolicyAppointController
        Inherits System.Web.Mvc.Controller


        '
        ' GET: /DeviceMDMConfig


        Function Index() As ActionResult
            Return View()
        End Function



        <HttpPost()>
        Function getPolicyProfile() As String
            Dim s As New DeviceMDMConfiguration
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", s.GetAllPolicyProfiles())
        End Function



        <HttpPost()>
        Function updateDevicePolicyAppointment()
            Dim s As New DeviceMDMConfiguration
            s.ID = Common.WebRequest.Data("ID")
            s.DevicePolicyProfileID = Common.WebRequest.Data("DevicePolicyProfileID")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            s.UpdateDevicePolicyAppointment()
        End Function






        <HttpPost()>
        Function getPolicyProfileIDByID() As String
            Dim s As New DeviceMDMConfiguration
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Return JsonDictionary.Serialize("Results", s.GetPolicyProfileIDByID())
        End Function








    End Class
End Namespace
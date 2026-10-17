Public Class EcommerceDataExporterController
    Inherits System.Web.Mvc.AsyncController

    Public Function StatusText() As String

        Return QuartzScheduler.EcommerceDataExporter.StatusText

    End Function

    Public Function IsBusy() As String

        Return QuartzScheduler.EcommerceDataExporter.IsBusy

    End Function

    Public Function Start() As String

        Dim pOrgID As String
        Dim pEcommerceOrderDataTargetServerIP As String
        Dim pError As String

        If QuartzScheduler.EcommerceDataExporter.IsBusy Then
            Return "busy"
        End If

#If DEBUG Then
        pOrgID = modClient.AbcOrgID
#Else
        pOrgID = modClient.DucOrgID
#End If

        WebRequest.Instance.OrgID = pOrgID
        If WebRequest.Instance.HandleOrganization(True, pError) = False Then
            Return pError
        End If

        pEcommerceOrderDataTargetServerIP = WebRequest.Instance.Business.SystemDefaults.EcommerceOrderDataTargetServerIP
        If String.IsNullOrWhiteSpace(pEcommerceOrderDataTargetServerIP) Then
            Return "No need to export data. EcommerceOrderDataTargetServerIP is not set. ref: vb!"
        End If
        pEcommerceOrderDataTargetServerIP = AppendIfNeeded(pEcommerceOrderDataTargetServerIP, "/")

        If QuartzScheduler.EcommerceDataExporter.IsBusy = False Then
            QuartzScheduler.EcommerceDataExporter.SaveMilestone(
                vPrivateDatabaseName:=WebRequest.Instance.ProData.GetCurrentDatabase(),
                vOrgID:=pOrgID,
                vPublicServerUrl:=pEcommerceOrderDataTargetServerIP)
            QuartzScheduler.TriggerEcommerceDataSyncJob()
        End If

        Return "started"

    End Function

    Public Function SaveSettings() As String

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        Dim pSettings As New EcommerceDataExporter.EcommerceDataExporterSettings
        pSettings.OrgID = WebRequest.Instance.OrgID
        pSettings.PrivateDatabaseName = WebRequest.Instance.ProData.GetCurrentDatabase()
        pSettings.PublicServerUrl = WebRequest.Instance.Business.SystemDefaults.EcommerceOrderDataTargetServerIP
        EcommerceDataExporter.SaveSettings(pSettings)

    End Function
End Class

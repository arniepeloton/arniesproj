
Namespace WebProj
    Public Class DeviceAppsWhiteListController
        Inherits System.Web.Mvc.Controller


        Function Index() As ActionResult
            Return View()
        End Function



        Function saveApplicationWhiteList() As String
            Dim s As New DeviceAppsWhiteList
            WebRequest.Instance.HandleOrganization(True) '= False Then Return WebRequest.InvalidOrganization
            s.ID = ""
            s.ID = WebRequest.Instance.Data("ID")
            s.ApplicationLabel = WebRequest.Instance.Data("ApplicationLabel")
            s.ApplicationName = WebRequest.Instance.Data("ApplicationName")
            s.Directory = WebRequest.Instance.Data("Directory")
            s.DevicePolicyProfileID = WebRequest.Instance.Data("DevicePolicyProfileID")
            Return JsonDictionary.Serialize("Results", s.SaveApplicationWhiteList)

        End Function





        Function getApplicationWhiteList() As String
            Dim clsRecord As New DeviceAppsWhiteList
            WebRequest.Instance.HandleOrganization(True)
            Return JsonDictionary.Serialize("DeviceAppsWhiteList", clsRecord.GetApplicationWhiteList())
        End Function


        Function getApplicationWhiteListByID() As String
            Dim clsRecord As New DeviceAppsWhiteList
            WebRequest.Instance.HandleOrganization(True)
            clsRecord.ID = WebRequest.Instance.Data("ID")
            Return JsonDictionary.Serialize("DeviceAppsWhiteList", clsRecord.GetApplicationWhiteListByID())
        End Function

        Function getApp() As ActionResult
            Return View()
        End Function



        Function getApplicationByAppID() As String
            Dim clsRecord As New DeviceAppsWhiteList
            WebRequest.Instance.HandleOrganization(True)
            clsRecord.ID = WebRequest.Instance.Data("ID")
            Return JsonDictionary.Serialize("DeviceAppsWhiteList", clsRecord.GetDeviceApplicationInfoByAppID(clsRecord.ID))
        End Function



        Function deleteWhiteApplicationByID() As String
            Dim clsRecord As New DeviceAppsWhiteList
            WebRequest.Instance.HandleOrganization(True)
            clsRecord.ID = WebRequest.Instance.Data("ID")
            clsRecord.DeleteDeviceWhiteAppByID()
        End Function

        

    End Class
End Namespace